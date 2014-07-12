#November 11 2013:
open1 = 1286.50
high1 = 1287.70
low1 = 1278.50
close1 = 1281.70

#November 12 2013
open2 = 1281.80
high2 = 1284.50
low2 = 1260.70
close2 = 1266.70

limitMove = 75

def SwingIndex(o1,o2,h1,h2,l1,l2,c1,c2,lm):

	def calc_K(h2,l2,c1):
		x = h2 - c1
		y = l2 - c1
	
		if x > y:
			k = x
			return k
		else:
			k = y
			return k

	def calc_R(h2,c1,l2,o1,lm):
		x = h2-c1
		y = l2-c1
		z = h2 - l2
		
		if z < x > y:
			R = (h2 - c1) - (.5 * (l2-c1)) + (.25 * (c1-o1))
			return R
			
		elif x < y > z:
			R = (l2 - c1) - (.5 * (h2-c1)) + (.25 * (c1-o1))
			return R
			
		elif x < z > y:
			R = (h2 - l2) + (.25 * (c1-o1))
			return R

	R = calc_R(h2, c1, l2, o1, lm)
	K = calc_K(h2,l2,c1)
	L = lm

	SwIn = 50 * ((c2-c1+(.5*(c2-o2))+(.25*(c1-o1)))/R) * (K/L)
	print SwIn
	return SwIn
		
SwingIndex(open1,open2,high1,high2,low1,low2,close1,close2,limitMove)
