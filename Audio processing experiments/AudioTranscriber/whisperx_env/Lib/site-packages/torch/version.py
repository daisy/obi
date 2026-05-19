from typing import Optional

__all__ = ['__version__', 'debug', 'cuda', 'git_version', 'hip', 'xpu']
__version__ = '2.8.0+cpu'
debug = False
cuda: Optional[str] = None
git_version = 'a1cb3cc05d46d198467bebbb6e8fba50a325d4e7'
hip: Optional[str] = None
xpu: Optional[str] = None
