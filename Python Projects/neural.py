from numpy import loadtxt
from keras.models import Sequential
from keras.layers import Dense

dataset = loadtxt('indians.csv', delimiter=',')

x = dataset[:,0:8]
y = dataset[:,8]

model = Sequential()
model.add(Dense(12, input_dim=8, activation='relu'))
model.add(Dense(12,activation='relu'))
model.add(Dense(8,activation='relu'))
model.add(Dense(8,activation='tanh'))
model.add(Dense(1,activation='sigmoid'))

model.compile(loss='binary_crossentropy', optimizer='adam', metrics=['accuracy'])

model.fit(x, y, epochs=150, batch_size=10)

accuracy = model.evaluate(x,y)
print('Accuracy: ',round(accuracy[1]*100, 2), '%')

print('------------------------------------------------')
print()
predictions = model.predict_classes(x)
for i in range(0,10):
	print('%s => %d (expected %d)' % (x[i].tolist(),predictions[i],y[i]))
