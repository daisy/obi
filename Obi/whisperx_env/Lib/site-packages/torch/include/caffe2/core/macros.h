// Automatically generated header file for caffe2 macros. These
// macros are used to build the Caffe2 binary, and if you are
// building a dependent library, they will need to be set as well
// for your program to link correctly.

#pragma once

#define CAFFE2_BUILD_SHARED_LIBS
/* #undef CAFFE2_FORCE_FALLBACK_CUDA_MPI */
/* #undef CAFFE2_HAS_MKL_DNN */
/* #undef CAFFE2_HAS_MKL_SGEMM_PACK */
#define CAFFE2_PERF_WITH_AVX
#define CAFFE2_PERF_WITH_AVX2
/* #undef CAFFE2_THREADPOOL_MAIN_IMBALANCE */
/* #undef CAFFE2_THREADPOOL_STATS */
/* #undef CAFFE2_USE_ACCELERATE */
/* #undef CAFFE2_USE_CUDNN */
/* #undef CAFFE2_USE_EIGEN_FOR_BLAS */
/* #undef CAFFE2_USE_FBCODE */
/* #undef CAFFE2_USE_GOOGLE_GLOG */
/* #undef CAFFE2_USE_LITE_PROTO */
#define CAFFE2_USE_MKL
#define USE_MKLDNN
/* #undef CAFFE2_USE_NVTX */
/* #undef CAFFE2_USE_ITT */

#ifndef EIGEN_MPL2_ONLY
#define EIGEN_MPL2_ONLY
#endif

// Useful build settings that are recorded in the compiled binary
// torch.__config__.show()
#define CAFFE2_BUILD_STRINGS { \
  {"TORCH_VERSION", "2.8.0"}, \
  {"CXX_COMPILER", "C:/actions-runner/_work/pytorch/pytorch/pytorch/.ci/pytorch/windows/tmp_bin/sccache-cl.exe"}, \
  {"CXX_FLAGS", "/DWIN32 /D_WINDOWS /EHsc /Zc:__cplusplus /bigobj /FS /utf-8 -DUSE_PTHREADPOOL -DNDEBUG -DUSE_KINETO -DLIBKINETO_NOCUPTI -DLIBKINETO_NOROCTRACER -DLIBKINETO_NOXPUPTI=ON -DUSE_FBGEMM -DUSE_XNNPACK -DSYMBOLICATE_MOBILE_DEBUG_HANDLE /wd4624 /wd4068 /wd4067 /wd4267 /wd4661 /wd4717 /wd4244 /wd4804 /wd4273"}, \
  {"BUILD_TYPE", "Release"}, \
  {"BLAS_INFO", "mkl"}, \
  {"LAPACK_INFO", "mkl"}, \
  {"USE_CUDA", "0"}, \
  {"USE_ROCM", "OFF"}, \
  {"CUDA_VERSION", ""}, \
  {"ROCM_VERSION", ""}, \
  {"USE_CUDNN", "OFF"}, \
  {"COMMIT_SHA", "a1cb3cc05d46d198467bebbb6e8fba50a325d4e7"}, \
  {"CUDNN_VERSION", ""}, \
  {"USE_NCCL", "OFF"}, \
  {"USE_MPI", "OFF"}, \
  {"USE_GFLAGS", "OFF"}, \
  {"USE_GLOG", "OFF"}, \
  {"USE_GLOO", "ON"}, \
  {"USE_NNPACK", "OFF"}, \
  {"USE_OPENMP", "ON"}, \
  {"FORCE_FALLBACK_CUDA_MPI", ""}, \
  {"HAS_MKL_DNN", ""}, \
  {"HAS_MKL_SGEMM_PACK", ""}, \
  {"PERF_WITH_AVX", "1"}, \
  {"PERF_WITH_AVX2", "1"}, \
  {"USE_ACCELERATE", ""}, \
  {"USE_EIGEN_FOR_BLAS", ""}, \
  {"USE_LITE_PROTO", ""}, \
  {"USE_MKL", "ON"}, \
  {"USE_MKLDNN", "ON"}, \
  {"USE_NVTX", ""}, \
  {"USE_ITT", ""}, \
  {"USE_ROCM_KERNEL_ASSERT", "OFF"}, \
  {"USE_CUSPARSELT", "OFF"}, \
  {"USE_XPU", "OFF"}, \
  {"USE_XCCL", "OFF"}, \
}
