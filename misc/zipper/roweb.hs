-- Read-only Web

module ROWeb where

data Term = Var String
          | Abs String Term
          | App Term Term
          | If Term Term Term
          deriving Show

rhs = Abs "n" (If (App (App (Var "=") (Var "n")) (Var "0"))
                  (Var "1")
                  (App (App (Var "+") (Var "n"))
                       (App (Var "fac") (App (Var "pred") (Var "n")))))

data Loc a = At { it   :: a,
                  down :: Loc a,
                  up   :: Loc a,
                  left :: Loc a,
                  right :: Loc a }

top   :: Term -> Loc Term
top t =  r where r = At t (weave r t) r r r

weave                  :: Loc Term -> Term -> Loc Term
weave l0 (Var s)       =  loc0 weave l0
weave l0 (Abs s t1)    =  loc1 weave l0 t1
weave l0 (App t1 t2)   =  loc2 weave l0 t1 t2
weave l0 (If t1 t2 t3) =  loc3 weave l0 t1 t2 t3

loc0 wv l0             =  l0
loc1 wv l0 t1          =  l1
  where l1             =  At t1 (wv l1 t1) l0 l1 l1
loc2 wv l0 t1 t2       =  l1
  where l1             =  At t1 (wv l1 t1) l0 l1 l2
        l2             =  At t2 (wv l2 t2) l0 l1 l2
loc3 wv l0 t1 t2 t3    =  l1
  where l1             =  At t1 (wv l1 t1) l0 l1 l2
        l2             =  At t2 (wv l2 t2) l0 l1 l3
        l3             =  At t3 (wv l3 t3) l0 l2 l3
