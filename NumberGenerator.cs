using System.Linq;
using UnityEngine;

public class NumberGenerator
{
    public int seed { get; set; }
    public int[][] gridOfRows { get; set; }
    public int[][] gridOfCols { get; set;}
    public int[][] gridOfVisibleBuildings { get; set; }
    private int countOfTries = 0;


    // Depricated
    public NumberGenerator(int count) {
        seed = GenerateSeed();
        gridOfRows = GenerateNumberGrid(count);
        gridOfCols = GenerateArrayOfColumns();
        gridOfVisibleBuildings = GenerateArrayOfVisibleBuildings();

        DebugMetod();
    }
    //-----------
    public NumberGenerator (int seed, int count) {
        this.seed = seed;
        gridOfRows = GenerateNumberGrid(count);
        gridOfCols = GenerateArrayOfColumns();
        gridOfVisibleBuildings = GenerateArrayOfVisibleBuildings();

        DebugMetod();
    }
    // public NumberGenerator (int[][] grid) {
    //     gridOfRows = grid;
    //     gridOfCols = GenerateArrayOfColumns();
    //     gridOfVisibleBuildings = GenerateArrayOfVisibleBuildings();

    //     DebugMetod();
    // }
    
    private int[][] GenerateNumberGrid(int count) {
        Random.InitState(seed);
        Debug.Log(seed);

        int[] arrayOfBuildingsTypes = new int[count];
        for (int i = 0; i < count; i++) {
            arrayOfBuildingsTypes[i] = i + 1;
        }

        int[][] grid = new int[count][];

        //Generating first row
        {
        int[] row = new int[count];
        int[] buildingsToAdd = new int[count];
        System.Array.Copy(arrayOfBuildingsTypes, 0, buildingsToAdd, 0, count);
        for (int j = 0; j < count; j++) {
            int index = Random.Range(0, buildingsToAdd.Length);
            row[j] = buildingsToAdd[index];
            DeleteIndexFromArray(index, ref buildingsToAdd);
        }
        
        grid[0] = row;
        }

        
        
        int brokenNum = 0;
        int limitOfTries = 20;
        for (int i = 1; i < count; i++) {
            bool last = false;
            int[] row = new int[count];
            int[] buildingsToAdd = new int[count];
            System.Array.Copy(arrayOfBuildingsTypes, 0, buildingsToAdd, 0, count);
            for (int j = 0; j < count; j++) {
                //Debug.Log(i + " " + j);  //For Debugging
                if (j == count - 1) {
                    last = true;
                }
                int[] localBuildingsToAdd = new int[buildingsToAdd.Length];
                System.Array.Copy(buildingsToAdd, 0, localBuildingsToAdd, 0, buildingsToAdd.Length);
                int[] indexesOfExistedNums = new int[0];/*----***----*/
                //Delete numbers which are already in the column
                for (int k = 0; k < i; k++) {
                    int existedNum = grid[k][j];
                    int indexOfExistedNum = System.Array.IndexOf(localBuildingsToAdd, existedNum);
                    if (indexOfExistedNum != -1) {
                        int kkk = indexesOfExistedNums.Length;/*---------********---------*/
                        System.Array.Resize(ref indexesOfExistedNums, kkk+ 1);/*-----------*******--------*/
                        indexesOfExistedNums[kkk] = indexOfExistedNum;/*-----------*******---------*/
                        DeleteIndexFromArray(indexOfExistedNum, ref localBuildingsToAdd);
                    }
                }
                //Works when the row generated wrong
                //The logic here is to regenerate the row anew using another Random.Range iteration
                if (brokenNum != 0) {
                    int indexOfBrokenNum = System.Array.IndexOf(localBuildingsToAdd, brokenNum);
                    DeleteIndexFromArray(indexOfBrokenNum, ref localBuildingsToAdd);

                    brokenNum = 0;
                }
                
                //This part trying to prevent some situation when numbers could have a wrong sequence
                if (!last) {
                    int[] generatedNumbers = new int[(count - j - 1) * i];
                    int indexOfExistedNums = 0;
                    int[] matches = new int[localBuildingsToAdd.Length];
                    int biggestMatch = 0;
                    int countOfBiggestMatches = 0;
                    int available = count - j;
                    bool stucked = false;
                    int[] numsWithMatches = new int[localBuildingsToAdd.Length];
                    int[] numsWithBiggestMatches = new int[localBuildingsToAdd.Length];
                    for (int k = 0; k < i; k++) {
                        for (int l = j + 1; l < count; l++) {
                            generatedNumbers[indexOfExistedNums] = grid[k][l];
                            indexOfExistedNums++;
                        }
                    }
                    for (int k = 0; k < localBuildingsToAdd.Length; k++) {
                        foreach (int l in generatedNumbers) {
                            if (l == localBuildingsToAdd[k]) {
                                matches[k] += 1;
                                numsWithMatches[k] = localBuildingsToAdd[k];
                            }
                        }
                    }
                    for (int l = 0; l < matches.Length; l++) {
                        if (matches[l] > biggestMatch) {
                                biggestMatch = matches[l];
                        }
                    }
                    if (biggestMatch > 0) {
                        for (int l = 0; l < matches.Length; l++) {
                            if (matches[l] == biggestMatch) {
                                numsWithBiggestMatches[countOfBiggestMatches] = numsWithMatches[l];
                                countOfBiggestMatches++;
                            }
                        }
                        if (countOfBiggestMatches == 1) {
                            if (biggestMatch == available - 1) {
                                row[j] = numsWithBiggestMatches[0];

                                int indexInBuildingsToAdd1 = System.Array.IndexOf(buildingsToAdd, row[j]);
                                DeleteIndexFromArray(indexInBuildingsToAdd1, ref buildingsToAdd);

                                continue;
                            }
                        }
                        //-------- This part could be easyly erased, but it will increase the number of "brokenNum" occurences -------------//
                        //-------- If you want to delete this part, you also have to delete lines marked as "/*---------*****---------*/" 
                        //------------------------------ Start of the part ----------------------------------------------------------------// 
                         else {
                            System.Array.Resize(ref numsWithBiggestMatches, countOfBiggestMatches);
                            for (int l = 0; l < biggestMatch - 1; l++) {
                                if (biggestMatch == available - l - 2) {
                                    stucked = true;
                                }
                            }
                            if (stucked && available - biggestMatch == countOfBiggestMatches) {
                                indexOfExistedNums = Random.Range(0, numsWithBiggestMatches.Length);
                                row[j] = numsWithBiggestMatches[indexOfExistedNums];

                                int indexInBuildingsToAdd1 = System.Array.IndexOf(buildingsToAdd, row[j]);
                                DeleteIndexFromArray(indexInBuildingsToAdd1, ref buildingsToAdd);

                                continue;
                            } else {
                                bool isItDone = false;
                                int[] indexesOfMatchedNums = new int[biggestMatch * countOfBiggestMatches];
                                int inde = 0;
                                for (int k = 0; k < i; k++) {
                                    foreach ( int l in numsWithBiggestMatches) {
                                        int convi = System.Array.IndexOf(grid[k], l);
                                        if (convi != -1 && convi > j) {
                                            indexesOfMatchedNums[inde] = convi;
                                            inde++;
                                        }
                                    }
                                }
                                float differenceIndexes = 0;
                                for (int k = 0; k < indexesOfMatchedNums.Length; k++) {
                                    foreach ( int l in indexesOfMatchedNums) {
                                        if (indexesOfMatchedNums[k] != l) {
                                            differenceIndexes++;
                                        }
                                    }
                                }
                                if (differenceIndexes != 0 && count - j -1 <= System.Math.Round(differenceIndexes/countOfBiggestMatches)) {
                                    foreach ( int l in indexesOfMatchedNums) {
                                        if (isItDone) break;
                                        foreach ( int k in indexesOfExistedNums) {
                                            if (isItDone) break;
                                            if (l == k) {
                                                for (int ii = 0; ii < i; ii++) {
                                                    if (isItDone) break;
                                                    foreach ( int ll in numsWithBiggestMatches){
                                                        int convice = System.Array.IndexOf(grid[ii], ll);
                                                        if (convice != -1 && convice > j && convice == l) {
                                                            row[j] = ll;
                                                            isItDone = true;

                                                            int indexInBuildingsToAdd11 = System.Array.IndexOf(buildingsToAdd, row[j]);
                                                            DeleteIndexFromArray(indexInBuildingsToAdd11, ref buildingsToAdd);

                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                     
                                } else if (differenceIndexes == 0 && countOfBiggestMatches == count - j - 1) {
                                    int randomIndex = Random.Range(0, numsWithBiggestMatches.Length);
                                    row[j] = numsWithBiggestMatches[randomIndex];

                                    int indexInBuildingsToAdd11 = System.Array.IndexOf(buildingsToAdd, row[j]);
                                    DeleteIndexFromArray(indexInBuildingsToAdd11, ref buildingsToAdd);

                                    continue;
                                }
                                if (isItDone) continue;
                            }
                        }
                        // ---------------------------------------------- End of the part ------------------------------------------------------ //

                    }
                }
                /*-------For Debugging--------
                // string matrix1 = "";
                // for (int ii = 0; ii < i; ii++) {
                //     matrix1 += '\t';
                //     if (ii == i ) {
                //         for (int jj = 0; jj < j; jj++) {
                //             matrix1 += grid[ii][jj];
                //         }
                //     } else {
                //         for (int jj = 0; jj < count; jj++) {
                //             matrix1 += grid[ii][jj];
                //         }
                //     }
                // }
                // string matrix2 = "";
                // for (int k = 0; k <= j; k++) {
                //     matrix2 += "" + row[k];
                // }
                // Debug.Log(matrix1 + '\t' + matrix2);
                --------------------------------*/
                if (localBuildingsToAdd.Length == 0) {
                    if (countOfTries == limitOfTries) {
                        Debug.Log("Broken Seed");
                    }
                    countOfTries++;
                    brokenNum = row[0];
                    i--;
                    break;
                }
                int index = Random.Range(0, localBuildingsToAdd.Length);
                row[j] = localBuildingsToAdd[index];
                
                
                int indexInBuildingsToAdd = System.Array.IndexOf(buildingsToAdd, row[j]);
                DeleteIndexFromArray(indexInBuildingsToAdd, ref buildingsToAdd);
            }
            if  (brokenNum == 0) grid[i] = row;
        }

        return grid;
    }

    public void DebugMetod() {
        string matrix= "";
        for (int i = 0; i < gridOfRows.Length; i ++) {
            matrix += '\t';
            for (int j = 0; j < gridOfRows[i].Length; j++) {
                matrix += gridOfRows[i][j];
            }
        }
        Debug.Log(matrix);

        int[][] newGrid = GenerateArrayOfColumns();
        Debug.Log(IsGridGeneratedNormally() + " " + countOfTries);

        int[][] gridOfNums = GenerateArrayOfVisibleBuildings();
        string matrix1= "";
        for (int i = 0; i < gridOfNums.Length; i ++) {
            matrix1 += '\t';
            for (int j = 0; j < gridOfNums[i].Length; j++) {
                matrix1 += gridOfNums[i][j];
            }
        }
        Debug.Log(matrix1);
    }



    // Depricated
    private int GenerateSeed() {
        System.Random rnd = new System.Random();
        int seed = rnd.Next(System.Int32.MinValue, System.Int32.MaxValue);
        return seed;
    }
    //-----------

    private void DeleteIndexFromArray (int indexToDelete, ref int[] array) {
        int[] buffer = new int[array.Length - 1];
        System.Array.Copy(array, 0, buffer, 0, indexToDelete);
        if (indexToDelete < array.Length - 1) {
            System.Array.Copy(array, indexToDelete + 1, buffer, indexToDelete, array.Length - indexToDelete - 1);
        }
        System.Array.Resize(ref array, array.Length - 1);
        System.Array.Copy(buffer, 0, array, 0, buffer.Length);
    }

    private int[][] GenerateArrayOfColumns() {
        int[][] gridOfColumns = new int[gridOfRows[0].Length][];
        for (int i = 0; i < gridOfRows.Length; i++) {
            int[] column = new int[gridOfRows.Length];
            for (int j = 0; j < gridOfRows.Length; j++)  {
                column[j] = gridOfRows[j][i];
            }
            gridOfColumns[i] = column;
        }
        return gridOfColumns;
    }

    public bool IsGridGeneratedNormally() {
        int count = gridOfCols.Length;
        int summ = 0;
        for (int i = 1; i <= count; i++) {
            summ += i;
        }
        foreach (int[] column in  gridOfCols) {
            if (summ != column.Sum()) return false;
        }
        return true;
    }

    // 1 row - left side; 2 row - right side
    // 3 row - top side; 4 row - bottom side
    private int[][] GenerateArrayOfVisibleBuildings() {
        int count = gridOfRows.Length;
        int[][] gridOfVisibleNums = new int[4][];
        int[][] processableGrid = gridOfRows;
        for (int i = 0; i < 4; i++) {
            int[] row = new int[count];
            int index = 0;
            int factor = 1;
            if (i == 1 || i == 3) {
                index = count -1;
                factor = -1;
            }
            if (i == 2) processableGrid = gridOfCols;
            for (int l = 0; l < processableGrid.Length; l++) {
                int[] column = processableGrid[l];
                int savedIndex = index;
                int biggestBuilding = 0;
                int visibleNum = 1;
                for (int j = 0; j < count ; j++) {
                    if (column[index] == count) {
                        break;
                    }
                    if (column[index] > biggestBuilding) {
                        biggestBuilding = column[index];
                        visibleNum++;
                    }
                    index += factor;
                }
                row[l] = visibleNum;
                index = savedIndex;         
            }
            gridOfVisibleNums[i] = row;
        }
        return gridOfVisibleNums;
    }
}