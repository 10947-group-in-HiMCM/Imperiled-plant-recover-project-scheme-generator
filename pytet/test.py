# -*- coding: utf-8 -*-

import io
import matplotlib.pyplot as plt
from scipy.optimize import curve_fit
import numpy as np
import socket
import operator

skt = socket.socket(socket.AF_INET,socket.SOCK_DGRAM)

skt.bind(('127.0.0.1',9090))

print('start');

def fitsolve(x, a, b, c):
    return a*np.log(b*x+c)

def process(data):
    try:
        srcdatastr = data.split(',')
        srcdata=[]
        for d in srcdatastr:
            srcdata.append(float(d));
        print(srcdata)
        xdata = np.linspace(0,len(srcdata),len(srcdata))
        popt, pcov = curve_fit(fitsolve, xdata, srcdata)
        res1 = popt
        res2 = pcov
        res3 = np.sqrt(np.diag(pcov))
        retdata = str(res1[0])+","+str(res1[1])+","+str(res1[2])
        return retdata
    except:
        return "0,0,0"

while True:

    data,addr = skt.recvfrom(1024)
    
    msg = data.decode('utf-8')
    msgarr = msg.split(':');

    if msgarr[0]=='quit':
        print('quit now')
        break;
        
    if msgarr[0]=='data':
        result = process(msgarr[1]);
        bmsg = result.encode()
        skt.sendto(bmsg,addr)

skt.close()





