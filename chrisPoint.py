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

def initGridStyle(stock):
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
    plt.setp(ax1.get_xticklabels(),visible=True)    
    plt.subplots_adjust(left=.09,bottom=.14,right=.94, top=.95, wspace=.20, hspace=0)
    for label in ax1.xaxis.get_ticklabels():
        label.set_rotation(45)    
    return (fig,ax1)

def getNumpyArray(MA2,stock):
    newAr = []
    conn = sqlite3.connect("penny.sqlite")
    cursor = conn.execute("SELECT time, open, close, high, low, volume FROM ticks where symbol = '{0}' and time like '%2014-06-24%' order by time".format(stock))
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
    SP = len(date[MA2-1:])
    return newAr,SP

stock = 'AHFD'    
(fig,ax1) = initGridStyle(stock)
newAr,SP = getNumpyArray(1,stock)
candlestick(ax1, newAr[-SP:], width=30/86400.0, colorup='#53c156', colordown='#ff1717')    
plt.show()
