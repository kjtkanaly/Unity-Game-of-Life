# Unity-Game-of-Life

Implementation of John Conway’s famous cellular automaton, the Game of Life. This project was used a way to work with compute shaders in Unity. Compute shaders are a helpful tool for running large simulations in Unity as they allow for computations to be done in parallel on the GPU. 

Running the game of life, with just the CPU restricts the resolution of the scene, but through the use of compute shaders we can increase the simulation resolution to modern resolutions (ex: 1920x1080). In these simulations, every pixel displayed to the user is a “cell” that has the potential to become alive. Thus, every generation of the “Life” algorithm requires us to consider every pixel on screen, which can quickly lead to high computation times depending on the simulation resolution.


[![IMAGE ALT TEXT HERE](https://img.youtube.com/vi/o3j11Yw2E3A/0.jpg)](https://www.youtube.com/watch?v=o3j11Yw2E3A)
