# vim: set fileencoding=UTF-8
# Zipper in python from Gérard Huet, "The Zipper", J. Functional Programming
# 7(5):549-554, Cambridge University Press, September 1997
# $Id$

class Item:
  """An item (leaf) in the tree; just a value."""

  def __init__(self, value):
    self.value = value

  def __repr__(self):
    return "Item(\"" + self.value + "\")"


class Section:
  """A section (node with children) in the tree; just a list of children."""

  def __init__(self, children):
    self.children = children

  def __repr__(self):
    return "Section([" + \
      ", ".join(map(lambda x: x.__repr__(), self.children)) + "])"


class Top:
  """Top of a path"""

  def __repr__(self):
    return "Top"


class Node:
  """Older (left) siblings, up path, younger siblings"""

  def __init__(self, left, up, right):
    self.left = left
    self.up = up
    self.right = right

  def __repr__(self):
    return "Node(" + \
      ", ".join(map(lambda x: x.__repr__(), [self.left, self.up, self.right])) \
      + ")"



class Loc:
  """type location = Loc of tree * path;;"""

  def __init__(self, tree, path):
    self.tree = tree
    self.path = path

  def __repr__(self):
    return "Loc(" + \
      ", ".join(map(lambda x: x.__repr__(), [self.tree, self.path])) + ")"

  def go_left(self):
    """Return the location to the left, or None if top- or leftmost."""
    if isinstance(self.path, Top) or len(self.path.left) == 0:
      return None
    else:
      return Loc(self.path.left[0],
                 Node(self.path.left[1:],
                      self.path.up,
                      [self.tree] + self.path.right))

  def go_right(self):
    """Return the location to the right, or None if top- or rightmost."""
    if isinstance(self.path, Top) or len(self.path.right) == 0:
      return None
    else:
      return Loc(self.path.right[0],
                 Node([self.tree] + self.path.left,
                      self.path.up,
                      self.path.right[1:]))

  def go_up(self):
    """Return the upper location, or None if topmost."""
    if isinstance(self.path, Top):
      return None
    else:
      return Loc(Section(list(self.path.left.__reversed__()) +
                         [self.tree] + self.path.right),
                 self.path.up)

  def go_down(self):
    """Return the lower location, or None if on an Item or an empty section."""
    if isinstance(self.tree, Item) or len(self.tree.children) == 0:
      return None
    else:
      return Loc(self.tree.children[0],
                 Node([], self.path, self.tree.children[1:]))

  def replace(self, tree):
    """Replace the tree at the current location with a new tree."""
    return Loc(tree, self.path)

  def insert_right(self, tree):
    """Insert a tree to the right (if not at top)"""
    if isinstance(self.path, Top):
      return None
    else:
      return Loc(self.tree,
                 Node(self.path.left, self.path.up, [tree] + self.path.right))

  def insert_left(self, tree):
    """Insert a tree to the left (if not at top)"""
    if isinstance(self.path, Top):
      return None
    else:
      return Loc(self.tree,
                 Node([tree] + self.path.left, self.path.up, self.path.right))

  def insert_down(self, tree):
    """Insert a tree down (if not a leaf)"""
    if isinstance(self.tree, Item):
      return None
    else:
      return Loc(tree, Node([], self.path, self.tree.children))

  def delete(self):
    """Delete at the current location"""
    if isinstance(self.path, Top):
      return None
    elif len(self.path.right) > 0:
      return Loc(self.path.right[0],
                 Node(self.path.left, self.path.up, self.path.right[1:]))
    elif len(self.path.left) > 0:
      return Loc(self.path.left[0],
                 Node(self.path.left[1:], self.path.up, []))
    else:
      return Loc(Section([]), self.path.up)


plus = Loc(Item("*"),
           Node([Item("c")],
                Node([Item("+"), Section([Item("a"), Item("*"), Item("b")])],
                     Top(), []),
                [Item("d")]))

print plus
print "LEFT: ", plus.go_left()
print "RIGHT: ", plus.go_right()
print "RIGHT, RIGHT: ", plus.go_right().go_right()
print "UP: ", plus.go_up()
print "UP, LEFT: ", plus.go_up().go_left()
print "DOWN: ", plus.go_down()
print "UP, DOWN: ", plus.go_up().go_down()
print "REPLACE(-): ", plus.replace(Item("-"))
print "LEFT, REPLACE(z): ", plus.go_left().replace(Item("z"))
print "INSERT_RIGHT(e): ", plus.insert_right(Item("e"))
print "INSERT_LEFT(e): ", plus.insert_left(Item("e"))
print "INSERT_DOWN(e): ", plus.insert_down(Item("e"))
print "UP, INSERT_DOWN(e): ", plus.go_up().insert_down(Item("e"))
print "DELETE: ", plus.delete()
