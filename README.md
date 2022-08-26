# Neural-Network-Experiments

A small experiment with plain neural networks in C# (inside the Unity engine for visualizations), created as part of [this video](https://youtu.be/hfMk-kjRv4c).
</br>The code has some minor differences from that shown in the video, for example the weights array has been flattened to a one dimensional array for performance (although the performance is admittedly still pretty poor).

The project contains 4 image recognition experiments: [mnist](http://yann.lecun.com/exdb/mnist/), [fashion mnist](https://github.com/zalandoresearch/fashion-mnist), [doodles](https://github.com/googlecreativelab/quickdraw-dataset), and [cifar10](https://www.cs.toronto.edu/~kriz/cifar.html).
<br>In the future I plan to build upon this to create a convolutional neural network, and potentially move the calculations to the GPU to speed things up.

![Doodles](https://raw.githubusercontent.com/SebLague/Images/master/Doodles.gif)
