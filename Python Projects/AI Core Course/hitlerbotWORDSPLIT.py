import torch
import numpy as np
import torch.nn.functional as F

device = torch.device("cuda:0" if torch.cuda.is_available() else "cpu")
print(device)

class CharRNNDataset():
    def __init__(self, txt_file_path='data/data.txt', chunk_size=50, transform=None):
        self.txt_file_path = txt_file_path
        self.chunk_size = chunk_size
        self.transform = transform
        
        #open our text file and read all the data into the rawtxt variable
        with open(txt_file_path, 'r') as file:
            rawtxt = file.read()

        #turn all of the text into lowercase as it will reduce the number of characters that our algorithm needs to learn
        rawtxt = rawtxt.lower()
        #words = rawtxt.split()
        
        letters = set(rawtxt) #returns the list of unique words in the raw text
        self.nchars = len(letters) #number of unique characters in our text file
        self.num_to_let = dict(enumerate(letters)) #created the dictionary mapping
        self.let_to_num = dict(zip(self.num_to_let.values(), self.num_to_let.keys())) #create the reverse mapping so we can map from a character to a unique number
        
        txt = list(rawtxt)#convert string to list
        for k, letter in enumerate(txt): #iterate through our text and change the value for each character to its mapped value
            txt[k] = self.let_to_num[letter] #set the kth item equal to the value it maps to

        self.X = np.array(txt) #convert txt to numpy array
    
    def __len__(self):
        return len(self.X)-1-self.chunk_size #the number of datapoints we have based on the chunk size and X
    
    def __getitem__(self, idx):
        x = self.X[idx:idx+self.chunk_size] #get the chunk at the particular index
        y = self.X[idx+1:idx+self.chunk_size+1] #get the labels which is like the input but shifted one to the left
        
        if self.transform: #apply the transform if any
            x, y = self.transform((x, y))
    
        return x, y


from torchvision import transforms
class ToLongTensor():
    def __init__(self):
        pass
    def __call__(self, inp):
        return (torch.LongTensor(var) for var in inp)


from torch.utils.data import DataLoader

batch_size = 16
chunk_size = 150 #the length of the sequences which we will optimize over

train_data = CharRNNDataset('data/data.txt', chunk_size=chunk_size, transform=ToLongTensor()) #instantiate dataset from class defined above
x, y = train_data[0]
x = x.to(device)
y = y.to(device)
print('First input', x)
print('First label', y, '\n')

nchars = train_data.nchars
num_to_let = train_data.num_to_let
let_to_num = train_data.let_to_num

print('Number of unique words:', nchars)
print('Length of dataset:', len(train_data))

train_loader = DataLoader(train_data,# make the training dataloader
                          batch_size = batch_size,
                          shuffle=True)



class CharRNN(torch.nn.Module):
    def __init__(self, input_size, embedding_len, hidden_size, output_size):
        super().__init__()
        self.hidden_size = hidden_size
        self.encoder = torch.nn.Embedding(input_size, embedding_len) #embedding layer
        self.i2h = torch.nn.Linear(embedding_len + hidden_size, hidden_size) #linear layer from I vector to the hidden
        self.h2y = torch.nn.Linear(hidden_size, output_size) #linear layer from hidden state to output

    def forward(self, x, hidden):
        embedding = self.encoder(x) #encode the input into a vector embedding
        combined = torch.cat((embedding, hidden), 1) #concatenate embedding and hidden to create I vector
        hidden = torch.tanh(self.i2h(combined)) #apply linear layer and activation function to calculate hidden state value
        output = self.h2y(hidden) #calculate output from hidden state
        #output = F.log_softmax(output, dim=1) #apply log softmax activation to output
        return output, hidden

    def init_hidden(self, x):
        return torch.zeros(x.shape[0], self.hidden_size) #zeros vector of hidden size for each input example



#hyper-params
lr = 0.001
epochs = 50
embedding_len = 400
hidden_size = 128

myRNN = CharRNN(nchars, embedding_len, hidden_size, nchars) #instantiate the model from the class defined earlier
criterion = torch.nn.CrossEntropyLoss() #define cost function - Cross Entropy Loss
optimizer = torch.optim.Adam(myRNN.parameters(), lr=lr) #choose optimizer

# SET UP TRAINING VISUALISATION
from torch.utils.tensorboard import SummaryWriter
writer = SummaryWriter() # we will use this to show our models performance on a graph

#training loop
def train(model, epochs):
    for epoch in range(epochs):
        epoch_loss = 0 #stores the cost for each epoch
        generated_string = '' #stores the text generated by our model for the 0th batch over the whole epoch
        for idx, (x, y) in enumerate(train_loader):
            x = x.to(device)
            y = y.to(device)
            loss = 0 #cost for this batch
            h = model.init_hidden(x).to(device) #initialize our hidden state to 0s
            for i in range(chunk_size): #sequentially input each character in the sequence for each batch and calculate loss
                out, h = model.forward(x[:, i], h) #calculate outputs based on input and previous hidden state
                
                _, outl = out.data.max(1) #based on our output, what character id does our network assign the highest probability of being next? # This is a [batch_size] sized Tensor
                    
                letter = num_to_let[outl[0].item()] #what chatacter is predicted for the 0th batch item?
                generated_string+=letter #+ " " #add the predicted letter to our generated sequence
                
                loss += criterion(out, y[:, i]) #add the cost for this input to the cost for the current batch
            
            writer.add_scalar('Loss/Train', loss/chunk_size, epoch*len(train_loader) + idx)    # write loss to a graph
            
            #based on the sum of the cost for this sequence (backpropagation through time) calculate the gradients and update our weights
            optimizer.zero_grad()
            loss.backward()
            optimizer.step()

            epoch_loss+=loss.item() #add the cost of this sequence to the cost of this epoch
        epoch_loss /= len(train_loader.dataset) #divide by the number of datapoinst in each epoch

        print('Epoch ', epoch+1, ' Avg Loss: ', epoch_loss)
        print('Generated text: ', generated_string[0:600], '\n')

myRNN = myRNN.to(device)
torch.backends.cudnn.benchmark=True
train(myRNN, epochs)


