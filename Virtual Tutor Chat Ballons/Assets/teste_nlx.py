#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Created on Wed Feb 28 21:59:41 2018

@author: balsaj
"""
import sys
import xmlrpc.client

def main():
    url = "http://lxsuite.variancia.com/"
    key = "joao.balsa.tv.23948273"

    proxy = xmlrpc.client.ServerProxy(url)

    frase = sys.argv[1]

    result = proxy.analyse(key, frase)
    print(result)

if __name__=='__main__':
    main()
