-- Read/write Web

module Web where

data Term = Var String
          | Abs String Term
          | App Term Term
          | If Term Term Term
          deriving Show

rhs = Abs "n" (If (App (App (Var "=") (Var "n")) (Var "0"))
                  (Var "1")
                  (App (App (Var "+") (Var "n"))
                       (App (Var "fac") (App (Var "pred") (Var "n")))))

data Loc a = At { it     :: a,
                  fdown  :: a -> Loc a,
                  fup    :: a -> Loc a,
                  fleft  :: a -> Loc a,
                  fright :: a -> Loc a }

down    :: Loc a -> Loc a
down l  =  (fdown l) (it l)

up      :: Loc a -> Loc a
up l    =  (fup l) (it l)

left    :: Loc a -> Loc a
left l  =  (fleft l) (it l)

right   :: Loc a -> Loc a
right l =  (fright l) (it l)

top t =  fr where fr t = At t (weave fr) fr fr fr

weave fl0 (Var s)       =  loc0 weave (fl0 (Var s))
weave fl0 (Abs s t1)    =  loc1 weave (\t1_ -> fl0 (Abs s t1_)) t1
weave fl0 (App t1 t2)   =  loc2 weave (\t1_ t2_ -> fl0 (App t1_ t2_)) t1 t2
weave fl0 (If t1 t2 t3) =  loc3 weave (\t1_ t2_ t3_ -> fl0 (If t1_ t2_ t3_))
                             t1 t2 t3

loc0 wv fl0_         =  fl0_
loc1 wv fl0_         =  fl1
  where fl1 t1       =  At t1 (wv (upd fl1)) (upd fl0_) (upd fl1) (upd fl1)
          where upd fl t1_ =  fl t1_
loc2 wv fl0_         =  fl1
  where fl1 t1  t2   =  At t1 (wv (upd fl1)) (upd fl0_) (upd fl1) (upd fl2)
           where upd fl t1_ =  fl t1_ t2
        fl2 t1 t2    =  At t2 (wv (upd fl2)) (upd fl0_) (upd fl1) (upd fl2)
           where upd fl t2_ =  fl t1 t2_
loc3 wv fl0_         =  fl1
  where fl1 t1 t2 t3 =  At t1 (wv (upd fl1)) (upd fl0_) (upd fl1) (upd fl2)
           where upd fl t1_ =  fl t1_ t2 t3
        fl2 t1 t2 t3 =  At t2 (wv (upd fl2)) (upd fl0_) (upd fl1) (upd fl3)
           where upd fl t2_ =  fl t1 t2_ t3
        fl3 t1 t2 t3 =  At t3 (wv (upd fl3)) (upd fl0_) (upd fl2) (upd fl3)
           where upd fl t3_ =  fl t1 t2 t3_

newtype Weaver a = W { unW :: (a -> Loc a) -> Loc a }

call wv fl0 t =  unW (wv t) fl0

con0 wv k          =  W (\fl0 -> loc0 (call wv) (fl0 k))
con1 wv k t1       =  W (\fl0 -> loc1 (call wv) (\t1_ -> fl0 (k t1_)) t1)
con2 wv k t1 t2    =  W (\fl0 -> loc2 (call wv)
                          (\t1_ t2_ -> fl0 (k t1_ t2_)) t1 t2)
con3 wv k t1 t2 t3 =  W (\fl0 -> loc3 (call wv)
                          (\t1_ t2_ t3_ -> fl0 (k t1_ t2_ t3_)) t1 t2 t3)

explore wv = fr where fr t = At t (call wv fr) fr fr fr
