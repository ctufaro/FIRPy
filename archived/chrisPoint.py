import sys, getopt
import urllib2
import time
import sqlite3
import datetime
import numpy as np
import matplotlib.pyplot as plt
import matplotlib.ticker as mticker
import matplotlib.dates as mdates
from matplotlib.finance import candlestick
import matplotlib
import pylab

green = '#53c156'
red = '#ff1717'
stock = None

def initTopGrid(sharedAxis):
    pass

def initMiddleGrid():
    #background color of main pane
    fig = plt.figure(facecolor = '#07000d')    
    #grid dimensions, sizing, and cell background color
    ax1 = plt.subplot2grid((6,4), (1,0), rowspan=4, colspan=4, axisbg='#07000d')    
    #overlay the grid
    ax1.grid(True, color='w')    
    #?
    ax1.xaxis.set_major_locator(mticker.MaxNLocator(10))
    ax1.xaxis.set_major_formatter(mdates.DateFormatter('%Y-%m-%d %H:%M:%S'))      
    #grid border colors
    ax1.spines['bottom'].set_color('#5998ff')
    ax1.spines['top'].set_color('#5998ff')
    ax1.spines['left'].set_color('#5998ff')
    ax1.spines['right'].set_color('#5998ff')    
    #x and y axis formatting
    ax1.tick_params(axis='y', colors='w')
    ax1.tick_params(axis='x', colors='w')    
    #?
    plt.gca().yaxis.set_major_locator(mticker.MaxNLocator(prune='upper'))    
    #set y axis text and color
    plt.ylabel('Stock Price')
    ax1.yaxis.label.set_color('w')      
    #set graph title
    plt.suptitle(stock.upper(),color='w')    
    #?
    plt.setp(ax1.get_xticklabels(),visible=False)    
    plt.subplots_adjust(left=.09,bottom=.14,right=.94, top=.95, wspace=.20, hspace=0)
    for label in ax1.xaxis.get_ticklabels():
        label.set_rotation(45)    
    return ax1
    
def initBottomGrid(sharedAxis):    
    ax2 = plt.subplot2grid((6,4), (5,0), sharex=sharedAxis, rowspan=1, colspan=4, axisbg='#07000d')      
    plt.gca().yaxis.set_major_locator(mticker.MaxNLocator(prune='upper'))
    plt.setp(ax2.get_xticklabels(),visible=False) 
    ax2.spines['bottom'].set_color("#5998ff")
    ax2.spines['top'].set_color("#5998ff")
    ax2.spines['left'].set_color("#5998ff")
    ax2.spines['right'].set_color("#5998ff")
    ax2.tick_params(axis='x', colors='w')
    ax2.tick_params(axis='y', colors='w')
    ax2.yaxis.set_major_locator(mticker.MaxNLocator(nbins=5, prune='upper'))
    return ax2
    
def getStockDataArray():
    newAr = []
    easyDate = []
    conn = sqlite3.connect("penny.sqlite")
    
    sql0 = "SELECT substr(time,0,11)[time], open,close,high,low,volume FROM ticks WHERE symbol = '{0}' GROUP BY symbol,substr(time,0,11) ORDER BY id".format(stock)
    sql0format = "%Y-%m-%d"
    
    sql1 = "SELECT time, open, close, high, low, volume FROM ticks where symbol = '{0}' and time like '%2014-06-26%' order by time".format(stock)
    sql1format="%Y-%m-%d %H:%M:%S"
    
    sql2 = "SELECT time, open, close, high, low, volume FROM ticks where symbol = '{0}' order by time".format(stock)
    sql2format="%Y-%m-%d %H:%M:%S"

    cursor = conn.execute(sql2)    
    for row in cursor:
        easyDate.append(row[0])
        newAr.append('{0},{1},{2},{3},{4},{5}'.format(row[0],row[1],row[2],row[3],row[4],row[5]))
    (date,openp, closep, highp, lowp,volume) = np.loadtxt(newAr,delimiter=',',unpack=True, converters={0:mdates.strpdate2num(sql2format)})
    conn.close()
    x = 0
    y = len(date)
    newAr = []
    while x < y:
        appendLine = date[x],float(openp[x]),float(closep[x]),float(highp[x]),float(lowp[x]),volume[x]
        newAr.append(appendLine)
        x += 1
    #SP = len(date[MA2-1:])
    return newAr,date,openp, closep, highp, lowp, volume, easyDate   
    
def getMovingAverage(values,window):
    weigths = np.repeat(1.0, window)/window
    smas = np.convolve(values, weigths, 'valid')
    return smas # as a numpy array    

def getSwingIndex(date, openp, closep, highp, lowp, LM):
    
    SwInY = []
    SwInDate = []
    x = 1
    
    def calc_R(H2,C1,L2,O1,LM):
        x = H2-C1
        y = L2-C1
        z = H2-L2

        if z < x > y:
            #print 'x wins!'
            R = (H2-C1)-(.5*(L2-C1))+(.25*(C1-O1))
            #print R
            return R
        elif x < y > z:
            #print 'y wins!'
            R = (L2-C1)-(.5*(H2-C1))+(.25*(C1-O1))
            #print R
            return R

        elif x < z > y:
            #print 'z wins!'
            R = (H2-L2)+(.25*(C1-O1))
            #print R
            return R


    def calc_K(H2,L2,C1):
        x = H2-C1
        y = L2-C1

        if x > y:
            K=x
            return K
        elif x < y:
            K=y
            return K
        else:
            return 0            
    
    while  x < len(date):        
        try:
            O1 = openp[x-1]
            O2 = openp[x]
            H1 = highp[x-1]
            H2 = highp[x]
            L1 = lowp[x-1]
            L2 = lowp[x]
            C1 = closep[x-1]
            C2 = closep[x]
            
            R = calc_R(H2,C1,L2,O1,LM)
            K = calc_K(H2,L2,C1)
            
            if R!=0:           
                SwIn = 50*((C2-C1+(.5*(C2-O2))+(.25*(C1-O1)))/R)*(K/LM)            
                SwInY.append(SwIn)
                SwInDate.append(date[x])
            
            x += 1
        except Exception as e:
            #print str(e)
            x += 1
        
    return SwInY,SwInDate

def getRSI(prices, date, n=10):
    deltas = np.diff(prices)
    seed = deltas[:n+1]
    up = seed[seed>=0].sum()/n
    down = -seed[seed<0].sum()/n
    rs = up/down
    rsi = np.zeros_like(prices)
    rsi[:n] = 100. - 100./(1.+rs)

    for i in range(n, len(prices)):
        delta = deltas[i-1] # cause the diff is 1 shorter

        if delta>0:
            upval = delta
            downval = 0.
        else:
            upval = 0.
            downval = -delta

        up = (up*(n-1) + upval)/n
        down = (down*(n-1) + downval)/n

        rs = up/down
        rsi[i] = 100. - 100./(1.+rs)
    return rsi    

def getCommandLineArgs():
    global stock
    if len(sys.argv) < 2:
        print 'Enter a stock'
        exit(4)
    else:
        stock = str((sys.argv)[1]) 
    
def plotMainCandleStick(ax1,newAr):
    candlestick(ax1, newAr[0:], width=30/86400.0, colorup=green, colordown=red) #30/86400.0
    
def plotGrid(axis,xdata,ydata,labelp):
    axis.plot(xdata,ydata,'#e1edf9',label=labelp, linewidth=1.5)
    
#init functions    
getCommandLineArgs() 
axisMiddle = initMiddleGrid()
axisTop = initTopGrid(axisMiddle)
axisBottom = initBottomGrid(axisMiddle)

#gathering indicator data
newAr, date, openp, closep, highp, lowp, volume, easyDate = getStockDataArray()
movingAverage = getMovingAverage(closep, 1)
SwInY,SwInDate = getSwingIndex(date, openp, closep, highp, lowp, 1)
rsi = getRSI(closep,easyDate)

#main plots
#plotMainCandleStick(axisMiddle,newAr)
#plotGrid(axisMiddle,date,movingAverage,'Moving Average')
#plotGrid(axisBottom,date,rsi,'RSI')
#plt.show()
