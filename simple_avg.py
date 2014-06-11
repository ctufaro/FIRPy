import numpy as np

dataset = [1,5,7,2,6,8,2,5,13,2,4]

def movingaverage(values,window):
	weights = np.repeat(1.0,window)/window
	smas = np.convolve(values, weights, 'valid')
	return smas
	
def expMovingAverage(values, window):
	weights = np.exp(np.linspace(-1.,0.,window))
	weights /= weights.sum()
	
	a = np.convolve(values,weights)[:len(values)]
	a[:window] = a[window]
	return a
	
print movingaverage(dataset,3)
print expMovingAverage(dataset,3)