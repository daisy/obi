-- Zipper in Haskell
-- From [Hinze and Jeuring, 2001]

module Zipper where

data Term = Var String
          | Abs String Term
          | App Term Term
          | If Term Term Term
          deriving Show

rhs = Abs "n" (If (App (App (Var "=") (Var "n")) (Var "0"))
                  (Var "1")
                  (App (App (Var "+") (Var "n"))
                       (App (Var "fac") (App (Var "pred") (Var "n")))))

data Loc = At { it :: Term, ctx :: Ctx } deriving Show
data Ctx = Top
         | Abs1 String Ctx
         | App1 Ctx Term
         | App2 Term Ctx
         | If1 Ctx Term Term
         | If2 Term Ctx Term
         | If3 Term Term Ctx
         deriving Show

top   :: Term -> Loc
top t =  At t Top

down                      :: Loc -> Loc
down (At (Var s) c)       =  At (Var s) c
down (At (Abs s t1) c)    =  At t1 (Abs1 s c)
down (At (App t1 t2) c)   =  At t1 (App1 c t2)
down (At (If t1 t2 t3) c) =  At t1 (If1 c t2 t3)

up                       :: Loc -> Loc
up (At t Top)            =  At t Top
up (At t1 (Abs1 s c))    =  At (Abs s t1) c
up (At t1 (App1 c t2))   =  At (App t1 t2) c
up (At t2 (App2 t1 c))   =  At (App t1 t2) c
up (At t1 (If1 c t2 t3)) =  At (If t1 t2 t3) c
up (At t2 (If2 t1 c t3)) =  At (If t1 t2 t3) c
up (At t3 (If3 t1 t2 c)) =  At (If t1 t2 t3) c

left                       :: Loc -> Loc
left (At t Top)            =  At t Top
left (At t1 (Abs1 s c))    =  At t1 (Abs1 s c)
left (At t1 (App1 c t2))   =  At t1 (App1 c t2)
left (At t2 (App2 t1 c))   =  At t1 (App1 c t2)
left (At t1 (If1 c t2 t3)) =  At t1 (If1 c t2 t3)
left (At t2 (If2 t1 c t3)) =  At t1 (If1 c t2 t3)
left (At t3 (If3 t1 t2 c)) =  At t2 (If2 t1 c t3)

right                       :: Loc -> Loc
right (At t top)            =  At t top
right (At t1 (Abs1 s c))    =  At t1 (Abs1 s c)
right (At t1 (App1 c t2))   =  At t2 (App2 t1 c)
right (At t2 (App2 t1 c))   =  At t2 (App2 t1 c)
right (At t1 (If1 c t2 t3)) =  At t2 (If2 t1 c t3)
right (At t2 (If2 t1 c t3)) =  At t3 (If3 t1 t2 c)
right (At t3 (If3 t1 t2 c)) =  At t3 (If3 t1 t2 c)
