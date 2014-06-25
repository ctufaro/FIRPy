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

def initTopGrid():
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
    conn = sqlite3.connect("penny.sqlite")
    sql = "SELECT time, open, close, high, low, volume FROM ticks where symbol = '{0}' and time like '%2014-06-25%' order by time".format(stock)
    cursor = conn.execute(sql)    
    for row in cursor:
        newAr.append('{0},{1},{2},{3},{4},{5}'.format(row[0],row[1],row[2],row[3],row[4],row[5]))
    (date,openp, closep, highp, lowp,volume) = np.loadtxt(newAr,delimiter=',',unpack=True, converters={0:mdates.strpdate2num("%Y-%m-%d %H:%M:%S")})
    conn.close()
    x = 0
    y = len(date)
    newAr = []
    while x < y:
        appendLine = date[x],float(openp[x]),float(closep[x]),float(highp[x]),float(lowp[x]),volume[x]
        newAr.append(appendLine)
        x += 1
    #SP = len(date[MA2-1:])
    return newAr,date,openp, closep, highp, lowp,volume   
    
def getMovingAverage(values,window):
    weigths = np.repeat(1.0, window)/window
    smas = np.convolve(values, weigths, 'valid')
    return smas # as a numpy array    

def getCommandLineArgs():
    global stock
    if len(sys.argv) < 2:
        print 'Enter a stock'
        exit(4)
    else:
        stock = str((sys.argv)[1]) 
    
def plotMainCandleStick(ax1,newAr):
    candlestick(ax1, newAr[0:], width=30/86400.0, colorup=green, colordown=red)
    
def plotGrid(axis,xdata,ydata,labelp):
    axis.plot(xdata,ydata,'#e1edf9',label=labelp, linewidth=1.5)
    
#init functions    
getCommandLineArgs() 
axisTop = initTopGrid()
axisMiddle = initMiddleGrid()
axisBottom = initBottomGrid(axisMiddle)

#gathering indicator data
newAr, date, openp, closep, highp, lowp, volume = getStockDataArray()
movingAverage = getMovingAverage(closep, 1)

#main plots
plotMainCandleStick(axisMiddle,newAr)
plotGrid(axisMiddle,date,movingAverage,'Moving Average')
plotGrid(axisBottom,date,movingAverage,'Moving Average')
plt.show()
