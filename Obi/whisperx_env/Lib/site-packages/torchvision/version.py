__version__ = '0.23.0+cpu'
git_version = '824e8c8726b65fd9d5abdc9702f81c2b0c4c0dc8'
from torchvision.extension import _check_cuda_version
if _check_cuda_version() > 0:
    cuda = _check_cuda_version()
