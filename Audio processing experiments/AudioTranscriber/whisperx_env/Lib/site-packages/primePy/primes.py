#This module returns primes factors (with multiplicity) of a number quickly.
#In addition, it can do several other operations including Euler totient funcion.
#run primes.about() to see the list of all functionality

#Written by Indrajit Jana. Suggestions are appreciated. Send those to ijana@temple.edu
from math import sqrt

def about():
    print("\'primes.check(n)\' returns \'True\' if \'n\' is a prime number")
    print("\'primes.factor(n)\' returns the lowest prime factor of \'n\'")
    print("\'primes.facors(n)\' returns all the prime factors of \'n\' with multiplicity")
    print("\'primes.first(n)\' returns first \'n\' many primes")
    print("\'primes.upto(n)\' returns all the primes less than or equal to \'n\'")
    print("\'primes.between(m,n)\' returns all the primes between \'m\' and \'n\'")
    print("\'primes.phi(n)\' returns the Euler's phi(n)")
    print("i.e., the number of integers less than n which have no common factor")

#Calculates the lowest prime factor by default
def factor(num):
    if num==2 or num%2==0:
        return 2
    else:
        for i in range(3, int(sqrt(num))+1, 2): #I could iteratte over a list of primes
            if num%i==0: #But creating that list of primes turns out even more inensive task
                return i 
        else:
            return num

def check(num):
    if factor(num)==num:
        return True
    else:
        return False

def factors(num):
    fact=factor(num)
    new_num=num//fact
    factors=[fact]
    while new_num!=1:
        fact=factor(new_num)
        factors+=[fact]
        new_num//=fact
    return factors

def phi(num):
    val=num
    list=factors(num) 
    sets=set(list) 
    for i in sets:
        val=(val//i)*(i-1)
    return val
        

def __next_prime(list): 
    if list==[2]:
        a=3
    else:
        a=list[-1]+2
        found=0 
        while found==0: 
            for i in list:
                if a%i==0:
                    a=a+1
                    break 
            else:
                found=1 
    return a


def first(n):
    list=[2]
    while len(list)<n:
        new_entry=__next_prime(list)
        list+=[new_entry]
    return list

def upto(n):
    list=[2]
    while list[-1]<n: 
        new_entry=__next_prime(list)
        list+=[new_entry]
    if list[-1]>n:
        list=list[:-1] 
    return list

def between(m,n):
    d=0
    x=[]
    if m%2==0:
        d=1
    else:
        d=0
    for i in range(m+d,n,2):
        if check(i):
            x+=[i]
        else:
            x=x
    return x