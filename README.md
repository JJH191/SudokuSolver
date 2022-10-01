# SudokuSolver
Sudoku Solver is a simple program that can take an image of a sudoku and solve it for you
<br /> <br />

## How It Works
1. You provide an image of a sudoku
2. It converts the image to black and white using an adjustable threshold
3. It tries to find the corners of the sudoku in the image. Sometimes it needs manual intervention here
4. The image is cropped and transformed so the sudoku is square and filling the whole image
5. This is then split into 81 individual cells, each of which are cleaned up (removing borders and artifacts)
6. The images are fed into a multi-layered neural network (written from scratch) for classification
7. The result is displayed in a grid, where it can be edited if there are any errors in the classification
8. Using a recursive back-propagation algorithm, the sudoku is solved

## Main Features
- Using a neural network to classify digits
- Solving sudokus
- Checking sudokus
- Saving previous sudokus for review

