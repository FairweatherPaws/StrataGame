using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GCScript : MonoBehaviour {
	
	public Transform tallHex, miniHex, seaFolder, landFolder, minimapFolder, hexplaneRiver, empty, categoryPrefab, waterfall, cameraTarget, cameraTLeft, cameraTRight, ready;
	public Material groundBase, seaDefault, saltSea, underWaterTile, grassTile, tundraTile, aridTile, desertTile, rockyTile, iceWall;
	public Material riverSpawn, riverStraight, riverBendLeft, riverBendRight, riverPond, riverMergeLeft, riverMergeRight, riverMergeSides, riverMergeAll;
	public Material forestMaterial, jungleMaterial, savannahMaterial, marshMaterial, wastelandMaterial;
	public Material mmSea, mmLake, mmIce, mmArid, mmDesert, mmGrass, mmRocky, mmTundra;
	public int sizeX = 30, sizeY = 20, iterations = 3, originalWeight = 6, seaLevel = 70, riverCount = 10, mountainRangeCount = 5, mountainRangeLength = 20, waterlevel = 0;
	public float heightIncrement = 0.05f;
	public List<Transform> allObjects = new List<Transform>();
	public bool runGenerate = true, landGen = true, mountainGen = false, landSmooth = false, seaGen = false, seaSpread = false, terrainGen = false, riverGen = false, catGen = false, minimapGen = false, spawnGen = false;
	private int mooseSize = 11, idNum = 0, xCoord = 1, yCoord = 2, elevation = 3, terrainType = 4, fertilityValue = 5, riverType = 6, riverDirection = 7, category = 8, waterType = 9, exists = 10;
	private int[,] moose;
	private float movementTicker = 0;
	/*
	 * 	private int[] miniMultiplier = {sizeX+1, sizeX-1, sizeX, 1, -1, -sizeX, -sizeX-1, -sizeX+1}; // array for scanning nearby hexes, 0-5 for even X coordinates, 2-7 for odd
	private int[] multiplier = {2*sizeX+1, 2*sizeX-1, 2*sizeX, sizeX+2, sizeX+1, sizeX, sizeX-1, sizeX-2, 2, 1, -1, -2, -sizeX+2, -sizeX+1, -sizeX, -sizeX-1, -sizeX-2, -2*sizeX, -2*sizeX-1, -2*sizeX+1}; // 2-hex array, even and odd similar to miniMultiplier
	private int[] hexSearchOdd = {sizeX, -1, -sizeX-1, -sizeX, -sizeX+1, 1};
	private int[] hexSearchEven = {sizeX, sizeX-1, -1, -sizeX, 1, sizeX+1};
	private int targetHex = 0;
	private int mooseSize = 8;
	private int area = sizeX * sizeY;
	private Material[] terrainTypes = {underWaterTile, aridTile, desertTile, grassTile, rockyTile, tundraTile};
	private Transform[] house = new Transform[area]; // pool of land tiles
	private Transform[] ponds = new Transform[area]; // pool of water tiles
	private int[,] moose = new int[area,mooseSize]; // land tile data array
	private int[,] waterMoose = new int[area,4]; // water tile data array
	private int jokin = 0; // percent underwater
	private int summa = 0;
	private int seaSize = 1;
	private int lakeRange = 18;
	private int[] fertilityArray = new int[area];
	private int riverTileCount = 0;
	private int[,] riverStart = new int[riverCount,2];
	private Transform[] riverSpawns = new Transform[riverCount];
	private List<Transform> allRivers = new List<Transform>();
	private List<Transform> allForests = new List<Transform>();
	private List<Transform> allJungles = new List<Transform>();
	private List<Transform> allSavannahs = new List<Transform>();
	private List<Transform> allIce = new List<Transform>();
	private List<Transform> allMarshes = new List<Transform>();
	private List<Transform> allWastelands = new List<Transform>();
	private Material[] mmArr = {mmSea, mmArid, mmDesert, mmGrass, mmRocky, mmTundra, mmIce, mmLake};
	*/
	// moose[tileid, x]
	// x = 0: tileid
	// x = 1: tile x-coord
	// x = 2: tile y-coord
	// x = 3: tile elevation
	// x = 4: tile terrain type {0: underWaterTile 0/3, 1: aridTile 2, 2: desertTile -3, 3: grassTile 5, 4: rockyTile -1, 5: tundraTile 1, 6: ICE --}
	// x = 5: tile fertility value
	// x = 6: river type: 0: no river, 1: start 2: straight river, 3: bendLeft, 4: bendRight, 5: bendLeft+straight, 6: bendRight+straight, 7: bendLeft+bendRight, 8: triple, 9: end pond;
	// x = 7: river direction: 0-5 from top clockwise
	// x = 8: category {0: nothing 1: ICE 2:forest, 3:jungle, 4:savannah, 5: marsh, 6: wasteland;
	// x = 9: water type: 0: not underwater, 1: lake, 2: sea

	void Awake () {
		moose = new int[sizeX*sizeY,mooseSize];
	}

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

		if (runGenerate) {
			int[] hexSearchOdd = {sizeX, -1, -sizeX-1, -sizeX, -sizeX+1, 1};
			int[] hexSearchEven = {sizeX, sizeX-1, -1, -sizeX, 1, sizeX+1};
			int[] hexLargeOdd = {2*sizeX, sizeX+2, sizeX+1, sizeX, sizeX-1, sizeX-2, 2, 1, -1, -2, -sizeX+2, -sizeX+1, -sizeX, -sizeX-1, -sizeX-2, -2*sizeX, -2*sizeX-1, -2*sizeX+1}; // 2-hex array, even and odd similar to miniMultiplier
			int[] hexLargeEven = {2*sizeX+1, 2*sizeX-1, 2*sizeX, sizeX+2, sizeX+1, sizeX, sizeX-1, sizeX-2, 2, 1, -1, -2, -sizeX+2, -sizeX+1, -sizeX, -sizeX-1, -sizeX-2, -2*sizeX}; // 2-hex array, even and odd similar to miniMultiplier
			int targetHex = 0;
			int area = sizeX * sizeY;
			Material[] terrainTypes = {underWaterTile, aridTile, desertTile, grassTile, rockyTile, tundraTile};
			Transform[] house = new Transform[area]; // pool of land tiles
			Transform[] ponds = new Transform[area]; // pool of water tiles
			 // land tile data array
			int[,] waterMoose = new int[area,4]; // water tile data array
			int jokin = 0; // percent underwater

			int seaSize = 1;
			int lakeRange = 18;
			int[] fertilityArray = new int[area];
			int riverTileCount = 0;
			int[,] riverStart = new int[riverCount,2];
			Transform[] riverSpawns = new Transform[riverCount];
			List<Transform> allRivers = new List<Transform>();
			List<Transform> allForests = new List<Transform>();
			List<Transform> allJungles = new List<Transform>();
			List<Transform> allSavannahs = new List<Transform>();
			List<Transform> allIce = new List<Transform>();
			List<Transform> allMarshes = new List<Transform>();
			List<Transform> allWastelands = new List<Transform>();
			Material[] mmArr = {mmSea, mmArid, mmDesert, mmGrass, mmRocky, mmTundra, mmIce, mmLake};
			int count = 0;
			int waterTileCount = area;
			Color colSwap = ready.renderer.material.color;

			if (landGen) {
				for (int i = 0; i < area; i++) { // give x and y coordinates to tiles
					if (i%sizeX == 0) {
						count++;
					}
					moose[i,idNum] = i;							// assign id number
					moose[i,xCoord] = i - sizeX * (count - 1);	// assign x coordinate
					moose[i,yCoord] = count - 1;				// assign y coordinate

					int roll = Random.Range (-100,101);			// random elevation
					moose[i,elevation] = roll;					// assign random value to elevation
					if (moose[i,elevation] > 70) { moose[i,elevation] += 10; }	// emphasise peaks,
					if (moose[i,elevation] > 100) { moose[i,elevation] = 100; } // cap at 100
				}
			}

			if (mountainGen) {
				for (int i = 0; i < mountainRangeCount; i++) {
					int mountainRangeRoll = Random.Range (2 * area/16, 14*area / 16);
					moose[mountainRangeRoll,elevation] = 100;
					int bias = Random.Range(0,6);
					int randomDirection = bias;
					int rangeLength = Random.Range (mountainRangeLength / 2, mountainRangeLength * 3 / 2);
					while (rangeLength > 0) {
						if (moose[mountainRangeRoll,1] % 2 == 0) {
							targetHex = hexSearchEven[randomDirection];
							if (mountainRangeRoll+targetHex > 0 && mountainRangeRoll+targetHex < area) {
								moose[mountainRangeRoll+targetHex,elevation] = 100;
								randomDirection = Random.Range (-1,1) + bias;
								
								if (randomDirection < 0) {
									randomDirection = 5; 
								}
								if (randomDirection > 5) {
									randomDirection = 0;
								}
								mountainRangeRoll += targetHex;
								rangeLength--;
							}
							else { 
								bias = Random.Range (0,6); 
								randomDirection = bias;
								continue; 
							}
						}
						else {
							targetHex = hexSearchOdd[randomDirection];
							if (mountainRangeRoll+targetHex > 0 && mountainRangeRoll+targetHex < area) {
								moose[mountainRangeRoll+targetHex,elevation] = 100;
								randomDirection = Random.Range (-1,1) + bias;
								if (randomDirection < 0) {
									randomDirection = 5; 
								}
								if (randomDirection > 5) {
									randomDirection = 0;
								}
								mountainRangeRoll += targetHex;
								rangeLength--;
							}
							else { 
								bias = Random.Range (0,6); 
								randomDirection = bias;
								continue; 
							}
						}
					}
				}
			}

			if (landSmooth) {
				for (int j = 0; j < iterations; j++) { // land height smoothing algorithm
					for (int i = 0; i < area; i++) {
						
						int sum = moose[i,elevation]*originalWeight;
						
						if (moose[i, elevation] > 50) { // weight on mountains
							sum += originalWeight*5;
						}
						if (moose[i, elevation] < -50) { // weight on depths
							sum -= originalWeight*5;
						}

						for (int k = 0; k < 6; k++) {
							if (moose[i,1] % 2 == 0) {
								targetHex = hexSearchEven[k];
								if (i+targetHex > 0 && i+targetHex < area && moose[i,elevation] != 100 && moose[i,elevation] != -100) {
									sum += moose[i+targetHex, elevation];
								}
							}
							else {
								targetHex = hexSearchOdd[k];
								if (i+targetHex > 0 && i+targetHex < area && moose[i,elevation] != 100 && moose[i,elevation] != -100) {
									sum += moose[i+targetHex, elevation];
								}
							}
						}
						sum /= originalWeight+6;
						moose[i, elevation] = sum;
						if (j == iterations - 1) {
							waterlevel += moose[i,elevation];
						}
					}
				}
			}
				
			if (seaGen) {
				waterlevel /= area;
				for ( int j = 0; j < 66; j++) { // sealevel setting alg.
					int checker = 0;
					for (int i = 0; i < area; i++) {
						if (moose[i,elevation] < waterlevel) {
							checker++;
							moose[i,waterType] = 2;
						}
						else {
							moose[i,waterType] = 0;
						}
					}
					jokin = checker * 100 / area;
					if (seaLevel > jokin) {
						waterlevel += 3;
					}
					if (seaLevel < jokin) {
						waterlevel -= 3;
					}
					if (jokin < seaLevel + 3 && jokin > seaLevel - 3) {
						break;
					}					
				}
				for (int i = 0; i < area; i++) { // checks to see if sea
					if (moose[i,waterType] > 0) {
						int surroundedByWater = 0;
						for (int j = 0; j < lakeRange; j++) {
							if (moose[i,xCoord] % 2 == 0) {
								targetHex = hexLargeEven[j];
								if (i+targetHex > 0 && i+targetHex < area && moose[i+targetHex,waterType] > 0) {
									surroundedByWater++;
								}
							}
							else {
								targetHex = hexLargeOdd[j];
								if (i+targetHex > 0 && i+targetHex < area && moose[i+targetHex,waterType] > 0) {
									surroundedByWater++;
								}
							}
						}
						
						if (surroundedByWater > lakeRange * 5 / 6) {
							int whatWhat = Random.Range(0, 5);
							if (whatWhat > 2) {moose[i,waterType] = 1;}
						}
					}
				}
			}

			if (seaSpread) {

				for (int j = 0; j < sizeX * 2; j++) { // spreads seas
					for (int i = 0; i < area; i++) {
						if (moose[i,waterType] == 1) {
							for (int k = 0; k < 6; k++) {
								if (moose[i,xCoord] % 2 == 0) {
									targetHex = hexSearchEven[k];
									if (i+targetHex > 0 && i+targetHex < area && moose[i+targetHex,waterType] == 2) {
										moose[i+targetHex,waterType] = 1;
									}
								}
								else {
									targetHex = hexSearchOdd[k];
									if (i+targetHex > 0 && i+targetHex < area && moose[i+targetHex,waterType] == 2) {
										moose[i+targetHex,waterType] = 1;
									}
								}
							}
						}
					}
				}
			}

			if (terrainGen) {
				for (int i = 0; i < area; i++) { // next up, temperature bands and elevation effects
					
					// random allocation of base tiles everywhere
					// 0 	-2, 
					// 1 	 4, 
					// 2 	-6, 
					// 3 	 10,
					// 4 	-4,
					// 5 	 2}
					int chulchul = Random.Range (0, 20);
					if (chulchul < 5) { 
						moose[i,terrainType] = 1; 
						moose[i,fertilityValue] = 4;
					}
					else if (chulchul > 18) { 
						moose[i,terrainType] = 2; 
						moose[i,fertilityValue] = -6;
					}
					else { 
						moose[i,terrainType] = 3; 
						moose[i,fertilityValue] = 10;
					}
					
					if (moose[i,yCoord] < sizeY * 2 / 16 || moose[i,yCoord] > sizeY * 14 / 16) { // tundra near poles
						moose[i,terrainType] = 5;
						moose[i,fertilityValue] = 2;
					}
					if (moose[i,yCoord] < sizeY * 9 / 16 && moose[i,yCoord] > sizeY * 7 / 16) { // equatorial
						int chulchula = Random.Range (0,20);
						if (chulchula < 5) { 
							moose[i,terrainType] = 1; 
							moose[i,fertilityValue] = 4;
						}
						else if (chulchula > 15) { 
							moose[i,terrainType] = 3; 
							moose[i,fertilityValue] = 10;
						}
						else { 
							moose[i,terrainType] = 2; 
							moose[i,fertilityValue] = -6;
						}
					}
					if (moose[i,elevation] > 45) { 
						moose[i,terrainType] = 4; 
						moose[i,fertilityValue] = -4;
					} // rocky type for peaks
				}


				for (int j = 0; j < iterations; j++) { // terrain smoother
					for (int i = 0; i < area; i++) {
						int which = Random.Range(-3,6);
						if (which > -1) { 
							for (int k = 0; k < 6; k++) {
								if (k == which) {
									if (moose[i,xCoord] % 2 == 0) {
										targetHex = hexSearchEven[k];
										if (i+targetHex > 0 && i+targetHex < area) {
											if (moose[i+targetHex,terrainType] > 0 && moose[i+targetHex,elevation] < 46) { 
												moose[i,terrainType] = moose[i+targetHex,terrainType];
											}
										}
									}
									else {
										targetHex = hexSearchOdd[k];
										if (i+targetHex > 0 && i+targetHex < area) {
											if (moose[i+targetHex,terrainType] > 0 && moose[i+targetHex,elevation] < 46) { 
												moose[i,terrainType] = moose[i+targetHex,terrainType];
											}
											
										}
									}
								}
							}
						}
						if (moose[i,elevation] > 45) { moose[i,terrainType] = 4; } // rocky type for peaks
						if (moose[i,waterType] > 0) { // underwater terrain
							moose[i,terrainType] = 0;
							if (moose[i,waterType] == 1) {
								moose[i,fertilityValue] = 0;
							}
							else {
								moose[i,fertilityValue] = 6;
							}
						}
						if (moose[i,terrainType] == 1) {
							moose[i,fertilityValue] = 4;
						}
						if (moose[i,terrainType] == 2) {
							moose[i,fertilityValue] = -6;
						}
						if (moose[i,terrainType] == 3) {
							moose[i,fertilityValue] = 10;
						}
						if (moose[i,terrainType] == 4) {
							moose[i,fertilityValue] = -4;
						}
						if (moose[i,terrainType] == 5) {
							moose[i,fertilityValue] = 2;
						}
						if (j == iterations - 1) {
							fertilityArray[i] = moose[i,fertilityValue];
							for (int k = 0; k < 6; k++) {
								if (moose[i,xCoord] % 2 == 0) {
									targetHex = hexSearchEven[k];
									if (i+targetHex > 0 && i+targetHex < area) {
										fertilityArray[i] += moose[i+targetHex,5] / 2;
									}
								}
								else {
									targetHex = hexSearchOdd[k];
									if (i+targetHex > 0 && i+targetHex < area) {
										fertilityArray[i] += moose[i+targetHex,5] / 2;
									}
								}
							}
						}
					}
				}
				for (int i = 0; i < area; i++) {
					moose[i,5] += fertilityArray[i];
				}
			}

			if (riverGen) { 
				int riverNumber = 0;
				int[] riverSpawner = new int[riverCount];
				for (int i = 100; i > -100; i--) {
					int riverTicker = 0;
					for (int j = 0; j < area; j++) {
						bool occupado = false;
						if (moose[j,elevation] > i  && moose[j,yCoord] < sizeY * 15 / 16 && moose[j,yCoord] > sizeY / 16) {
							for (int k = 0; k < 6; k++) {
								if (moose[j,xCoord] % 2 == 0) {
									targetHex = hexSearchEven[k];
									if (j+targetHex > 0 && j+targetHex < area) {
										if (moose[j+targetHex,riverType] == 1) { 
											occupado = true;
										}
									}
								} 
								else {
									targetHex = hexSearchOdd[k];
									if (j+targetHex > 0 && j+targetHex < area) {
										if (moose[j+targetHex,riverType] == 1) { 
											occupado = true;
										}
									}
								}
							}
							if (riverTicker < riverCount && !occupado) {
								riverSpawner[riverTicker] = j;
								moose[j,riverType] = 1;
								riverTicker++;
							}
							else {
								moose[j,riverType] = 0;
							}
						}
					}
					if (riverTicker >= riverCount) {
						break;
					}
				}
				for (int i = 0; i < riverCount; i++) {
					Debug.Log (riverSpawner[i]);
					int oldDirection = -1;
					int direction = -1;
					while (true) {
						oldDirection = direction;
						direction = -1;
						int lowest = moose[riverSpawner[i],elevation];
						for (int j = 0; j < 6; j++) {
							if (moose[riverSpawner[i],xCoord] % 2 == 0) {
								targetHex = hexSearchEven[j];
								if (riverSpawner[i]+targetHex > 0 && riverSpawner[i]+targetHex < area) {
									if (moose[riverSpawner[i]+targetHex,elevation] < lowest) { 
										direction = j;
										lowest = moose[riverSpawner[i]+targetHex,elevation];
									}
								}
							} 
							else {
								targetHex = hexSearchOdd[j];
								if (riverSpawner[i]+targetHex > 0 && riverSpawner[i]+targetHex < area) {
									if (moose[riverSpawner[i]+targetHex,elevation] < lowest) { 
										direction = j;
										lowest = moose[riverSpawner[i]+targetHex,elevation];
									}
								}
							}
						}
						// x = 6: river type: 0: no river, 1: start 2: straight river, 3: bendLeft, 4: bendRight, 
						// 5: bendLeft+straight, 6: bendRight+straight, 7: bendLeft+bendRight, 8: triple, 9: end pond;
						if (direction > -1) {
							moose[riverSpawner[i],riverDirection] = direction;
							if (moose[riverSpawner[i],xCoord] % 2 == 0) {
								if (moose[riverSpawner[i],riverType] != 1) {
									if ((oldDirection == direction - 1) || (oldDirection == direction + 5)) {
										if (moose[riverSpawner[i],riverType] == 0) {
											moose[riverSpawner[i],riverType] = 4;
										}
										if (moose[riverSpawner[i],riverType] == 2) {
											moose[riverSpawner[i],riverType] = 6;
										}
										if (moose[riverSpawner[i],riverType] == 3) {
											moose[riverSpawner[i],riverType] = 7;
										}
										if (moose[riverSpawner[i],riverType] == 5) {
											moose[riverSpawner[i],riverType] = 8;
										}
									}
									if ((oldDirection == direction + 1) || (oldDirection == direction - 5)) {
										if (moose[riverSpawner[i],riverType] == 0) {
											moose[riverSpawner[i],riverType] = 3;
										}
										if (moose[riverSpawner[i],riverType] == 2) {
											moose[riverSpawner[i],riverType] = 5;
										}
										if (moose[riverSpawner[i],riverType] == 4) {
											moose[riverSpawner[i],riverType] = 7;
										}
										if (moose[riverSpawner[i],riverType] == 6) {
											moose[riverSpawner[i],riverType] = 8;
										}
									}
									if (oldDirection == direction) {
										if (moose[riverSpawner[i],riverType] == 0) {
											moose[riverSpawner[i],riverType] = 2;
										}
										if (moose[riverSpawner[i],riverType] == 3) {
											moose[riverSpawner[i],riverType] = 5;
										}
										if (moose[riverSpawner[i],riverType] == 4) {
											moose[riverSpawner[i],riverType] = 6;
										}
										if (moose[riverSpawner[i],riverType] == 7) {
											moose[riverSpawner[i],riverType] = 8;
										}
									}
								}
								riverSpawner[i] = riverSpawner[i]+hexSearchEven[direction];
							} 
							else {
								if (moose[riverSpawner[i],riverType] != 1) {
									if ((oldDirection == direction - 1) || (oldDirection == direction + 5)) {
										if (moose[riverSpawner[i],riverType] == 0) {
											moose[riverSpawner[i],riverType] = 4;
										}
										if (moose[riverSpawner[i],riverType] == 2) {
											moose[riverSpawner[i],riverType] = 6;
										}
										if (moose[riverSpawner[i],riverType] == 3) {
											moose[riverSpawner[i],riverType] = 7;
										}
										if (moose[riverSpawner[i],riverType] == 5) {
											moose[riverSpawner[i],riverType] = 8;
										}
									}
									if ((oldDirection == direction + 1) || (oldDirection == direction - 5)) {
										if (moose[riverSpawner[i],riverType] == 0) {
											moose[riverSpawner[i],riverType] = 3;
										}
										if (moose[riverSpawner[i],riverType] == 2) {
											moose[riverSpawner[i],riverType] = 5;
										}
										if (moose[riverSpawner[i],riverType] == 4) {
											moose[riverSpawner[i],riverType] = 7;
										}
										if (moose[riverSpawner[i],riverType] == 6) {
											moose[riverSpawner[i],riverType] = 8;
										}
									}
									if (oldDirection == direction) {
										if (moose[riverSpawner[i],riverType] == 0) {
											moose[riverSpawner[i],riverType] = 2;
										}
										if (moose[riverSpawner[i],riverType] == 3) {
											moose[riverSpawner[i],riverType] = 5;
										}
										if (moose[riverSpawner[i],riverType] == 4) {
											moose[riverSpawner[i],riverType] = 6;
										}
										if (moose[riverSpawner[i],riverType] == 7) {
											moose[riverSpawner[i],riverType] = 8;
										}
									}
								}
								riverSpawner[i] = riverSpawner[i]+hexSearchOdd[direction];
							}
						}
						else {
							moose[riverSpawner[i],riverType] = 9;
							break;
						}
					}
				}

			}


//			if (riverGen) {
//				for (int i = 0; i < area; i++) {
//					int lowest = riverStart[0,1];
//					int lowestId = 0;
//					for (int j = 0; j < riverCount; j++) {
//						if (riverStart[j,1] < lowest) {
//							lowestId = j;
//							lowest = riverStart[j,1];
//						}
//					}
//		
//					if (moose[i,elevation] > riverStart[lowestId,1]) {
//						bool notThere = true;
//						for (int j = 0; j < 6; j++) {
//							for (int k = 0; k < riverCount; k++) {
//								if (moose[i,xCoord] % 2 == 0) {
//									targetHex = hexSearchEven[j];
//									if (i+targetHex > 0 && i+targetHex < area) {
//										if (riverStart[k,0] == i+targetHex) { 
//											notThere = false;
//										}
//									}
//								}
//								else {
//									targetHex = hexSearchOdd[j];
//									if (i+targetHex > 0 && i+targetHex < area) {
//										if (riverStart[k,0] == i+targetHex) { 
//											notThere = false;
//										}
//									}
//								}
//							}
//						}
//						if (notThere) {
//							riverStart[lowestId, 0] = i;
//							riverStart[lowestId, 1] = moose[i,elevation];
//						}
//					}
//				}
//
//				for (int i = 0; i < riverCount; i++) {
//					int direction = 0;
//					int lowest = riverStart[i,1];
//					int target = riverStart[i,0];
//					int current = riverStart[i,0];
//					moose[current,riverType] = 1;
//					for (int j = 0; j < 6; j++) {
//						if (moose[target,xCoord] % 2 == 0) {
//							targetHex = hexSearchEven[j];
//							if (target+targetHex > 0 && target+targetHex < area) {
//								if (moose[target+targetHex,3] < lowest) { 
//									lowest = moose[target+targetHex,elevation];
//									target = moose[target+targetHex,idNum];
//									direction = j;
//								}
//							}
//						}
//						else {
//							targetHex = hexSearchOdd[j];
//							if (target+targetHex > 0 && target+targetHex < area) {
//								if (moose[target+targetHex,3] < lowest) { 
//									lowest = moose[target+targetHex,elevation];
//									target = moose[target+targetHex,idNum];
//									direction = j;
//								}
//							}
//						}
//					}
//					moose[current,riverDirection] = direction;
//				}
//
//				for (int i = 0; i < riverCount; i++) {
//					bool running = true;
//					int countise = 0;
//					int direction = 0;
//					int oldDirection = 0;
//					int lowest = riverStart[i,1];
//					int target = riverStart[i,0];
//					while (running) {
//						int current = target;
//						oldDirection = direction;
//						direction = -1;
//						for (int j = 0; j < 6; j++) {
//							if (moose[current,xCoord] % 2 == 0) {
//								targetHex = hexSearchEven[j];
//								if (current+targetHex > 0 && current+targetHex < area) {
//									if (moose[current+targetHex,elevation] < lowest) { 
//										lowest = moose[current+targetHex,elevation];
//										target = moose[current+targetHex,idNum];
//										direction = j;
//									}
//								}
//							}
//							else {
//								targetHex = hexSearchOdd[j];
//								if (current+targetHex > 0 && current+targetHex < area) {
//									if (moose[current+targetHex,elevation] < lowest) { 
//										lowest = moose[current+targetHex,elevation];
//										target = moose[current+targetHex,idNum];
//										direction = j;
//									}
//								}
//							}
//						}
//						if (direction == -1) {
//							moose[current, riverType] = 9;
//							running = false;
//							break;
//						}
//						if (moose[target,riverType] > 1) {
//							if (moose[target,riverType] == 5 || moose[target,riverType] == 6 || moose[target,riverType] == 7) {
//								moose[target,riverType] = 8;
//							}
//							if (moose[target,riverType] == 2) {
//								if (moose[target,riverDirection] == direction + 1 || moose[target,riverDirection] == direction - 5) {
//									moose[target,riverType] = 6;
//								}
//								if (moose[target,riverDirection] == direction - 1 || moose[target,riverDirection] == direction + 5) {
//									moose[target,riverType] = 5;
//								}
//							}
//							if (moose[target,riverType] == 3) {
//								if (moose[target,riverDirection] == direction) {
//									moose[target,riverType] = 5;
//								}
//								if (moose[target,riverDirection] == direction - 1 || moose[target,riverDirection] == direction + 5) {
//									moose[target,riverType] = 7;
//								}
//							}
//							if (moose[target,riverType] == 4) {
//								if (moose[target,riverDirection] == direction) {
//									moose[target,riverType] = 6;
//								}
//								if (moose[target,riverDirection] == direction + 1 || moose[target,riverDirection] == direction - 5) {
//									moose[target,riverType] = 7;
//								}
//							}
//						}
//						if (direction == oldDirection + 1 || direction == oldDirection - 5) {
//							moose[target,riverType] = 3;
//							moose[target,riverDirection] = direction;
//						}
//						if (direction == oldDirection - 1 || direction == oldDirection + 5) {
//							moose[target,riverType] = 4;
//							moose[target,riverDirection] = direction;
//						}
//						if (direction == oldDirection) {
//							moose[target,riverType] = 2;
//							moose[target,riverDirection] = direction;
//						}
//						if (waterlevel > moose[target, elevation]) {
//							running = false;
//						}
//					}
//				}
//			}

			if (catGen) {
				for (int i = 0; i < area; i++) {
					if (moose[i,yCoord] > sizeY * 15 / 16 || moose[i,yCoord] < sizeY / 16) { //icecaps
						if (moose[i,elevation] < 40) {
							moose[i,elevation] = 40;
							moose[i,terrainType] = 6;
							moose[i,category] = 1;
						}
					}
					if (moose[i,waterType] == 0) { // land only
						if (moose[i,riverType] == 9) {
							int roller = Random.Range (0, 20);
							if (roller < 4 && moose[i,yCoord] < sizeY * 13 / 16 && moose[i,yCoord] > sizeY * 3 / 16) { //marsh
								moose[i, category] = 5;
								moose[i, fertilityValue] -= 40;
							}
						}
						if (moose[i,riverType] > 0) {
							moose[i, fertilityValue] += 20;
						}

						if (3 > Random.Range (0,100) && moose[i,terrainType] % 2 == 1) { //wasteland
							moose[i,category] = 6;
							moose[i,fertilityValue] -= 40;
						}
						if (moose[i,fertilityValue] > 40 && moose[i,terrainType] == 3 && moose[i,category] == 0) {
							if (moose[i,yCoord] > sizeY * 6 / 16 && moose[i,yCoord] < sizeY * 10 / 16) {
								moose[i,category] = 3;
								moose[i,fertilityValue] += 20;
							}
							else {
								moose[i,category] = 2;
								moose[i,fertilityValue] += 10;
							}
						}
						if (moose[i,fertilityValue] > 20 && moose[i,terrainType] == 1 && moose[i,category] == 0) {
							moose[i,category] = 4;
							moose[i,fertilityValue] += 10;
						}
					}
				}
			}				
//			if (minimapGen) {
//				for (int i = 0; i < area; i++) {
//					float add = 0f;
//					if (moose[i,xCoord] % 2 == 1) { 
//						add += 0.57f;
//					}
//					Transform gotme = Instantiate (categoryPrefab, new Vector3(moose[i,xCoord], moose[i,yCoord]*1.15f - add - 1000, Random.Range (-1000,1000)*0.01f), Quaternion.identity) as Transform;                                                                       
//					gotme.localScale = new Vector3(1f,1.1f,1f);
//					gotme.transform.Rotate(0,0,30);
//					gotme.renderer.material = mmArr[moose[i,terrainType]];
//					if (moose[i,terrainType] == 0) {
//						if (moose[i,waterType] == 2) {
//							gotme.renderer.material = mmLake;
//						}
//						else {
//							gotme.renderer.material = mmSea;
//						}
//					}
//				}
//			}
			if (minimapGen) {
				spawnGen = true;
				minimapGen = false;
				runGenerate = false;
			}
			if (catGen) {
				ready.renderer.material.mainTextureOffset = new Vector2(0.55f, 0);
				minimapGen = true;
				catGen = false;
			}
			if (riverGen) {
				ready.renderer.material.mainTextureOffset = new Vector2(0.61f, 0);
				catGen = true;
				riverGen = false;
			}
			if (terrainGen) {
				ready.renderer.material.mainTextureOffset = new Vector2(0.67f, 0);
				riverGen = true;
				terrainGen = false;
			}
			if (seaSpread) {
				ready.renderer.material.mainTextureOffset = new Vector2(0.73f, 0);
				terrainGen = true;
				seaSpread = false;
			}
			if (seaGen) {
				ready.renderer.material.mainTextureOffset = new Vector2(0.79f, 0);
				seaSpread = true;
				seaGen = false;
			}
			if (landSmooth) {
				ready.renderer.material.mainTextureOffset = new Vector2(0.85f, 0);
				seaGen = true;
				landSmooth = false;
			}
			if (mountainGen) {
				ready.renderer.material.mainTextureOffset = new Vector2(0.91f, 0);
				landSmooth = true;
				mountainGen = false;
			}
			if (landGen) {
				ready.renderer.material.mainTextureOffset = new Vector2(0.97f, 0);
				mountainGen = true;
				landGen = false;
			}
		}
		if (spawnGen) {
//		if (Input.GetAxis ("Horizontal") != 0 || Input.GetAxis ("Vertical") != 0) {
			Material[] terrainTypes = {saltSea, aridTile, desertTile, grassTile, rockyTile, tundraTile, iceWall, seaDefault};
			Material[] riverTypes = {null, riverSpawn, riverStraight, riverBendLeft, riverBendRight, riverMergeLeft, riverMergeRight, riverMergeSides, riverMergeAll, riverPond};
			Material[] catTypes = {forestMaterial, jungleMaterial, savannahMaterial, marshMaterial, wastelandMaterial};
			for ( int i = 0; i < sizeX*sizeY; i++) {
				float aox = moose[i,xCoord];
				float aoy = moose[i,yCoord];
				float ctx = cameraTarget.transform.position.x;
				float cty = cameraTarget.transform.position.y;
				float clx = cameraTLeft.transform.position.x;
				float cly = cameraTLeft.transform.position.y;
				float crx = cameraTRight.transform.position.x;
				float cry = cameraTRight.transform.position.y;
				if( aox + 15 > ctx && aox - 15 < ctx && aoy + 15 > cty && aoy - 15 < cty ||
				    aox + 15 > clx && aox - 15 < clx && aoy + 15 > cly && aoy - 15 < cly ||
				    aox + 15 > crx && aox - 15 < crx && aoy + 15 > cry && aoy - 15 < cry) {
					if (moose[i,exists] == 0) {
						float add = 0f;
						if (moose[i,xCoord] % 2 == 1) { add += 0.57f; }
						Transform hex = Instantiate(tallHex, new Vector3(aox, aoy*1.15f - add, moose[i,elevation]*heightIncrement), Quaternion.identity) as Transform;
						if (moose[i,waterType] > 0 && moose[i,terrainType] != 6) {
							hex.transform.position = new Vector3(hex.transform.position.x, hex.transform.position.y, waterlevel * heightIncrement);
							if (moose[i,waterType] == 2) {
								hex.renderer.material = terrainTypes[7];
							}
							else {
								hex.renderer.material = terrainTypes[0];
							}
						}
						else {
							hex.renderer.material = terrainTypes[moose[i,terrainType]];
						}
						if (moose[i,riverType] > 0) {
							Transform riverHex = Instantiate(categoryPrefab, new Vector3(aox, aoy*1.15f - add, moose[i,elevation]*heightIncrement + 3.4f), Quaternion.identity) as Transform;
							riverHex.renderer.material = riverTypes[moose[i,riverType]];
							riverHex.transform.parent = hex;
							riverHex.transform.Rotate (0,0,-120 + 60*moose[i,riverDirection]);
							riverHex.localScale = new Vector3(1f,1.1f,1f);
						}
						if (moose[i, category] > 1) {
							Transform catHex = Instantiate(categoryPrefab, new Vector3(aox, aoy*1.15f - add, moose[i,elevation]*heightIncrement + 3.45f), Quaternion.identity) as Transform;
							catHex.renderer.material = catTypes[moose[i,category]-2];
							catHex.transform.parent = hex;
							catHex.transform.Rotate (0,0,Random.Range(0, 360));
							catHex.localScale = new Vector3(1f,1.1f,1f);
						} 
						hex.transform.Rotate(0,0,30);
						allObjects.Add (hex);
						hex.gameObject.AddComponent<HexData>();
						hex.gameObject.GetComponent<HexData>().idNum = i;
						moose[i,exists] = 1;
					}
				}
			}
			spawnGen = false;
			Destroy (ready.gameObject);
		}
		if (Input.GetAxis ("Horizontal") != 0 || Input.GetAxis ("Vertical") != 0) {
			movementTicker += Time.deltaTime;
//		}
//		if (movementTicker > 0.1f) {
			Material[] terrainTypes = {saltSea, aridTile, desertTile, grassTile, rockyTile, tundraTile, iceWall, seaDefault};
			Material[] riverTypes = {null, riverSpawn, riverStraight, riverBendLeft, riverBendRight, riverMergeLeft, riverMergeRight, riverMergeSides, riverMergeAll, riverPond};
			Material[] catTypes = {forestMaterial, jungleMaterial, savannahMaterial, marshMaterial, wastelandMaterial};
			float ctx = cameraTarget.transform.position.x;
			float cty = cameraTarget.transform.position.y;
			float clx = cameraTLeft.transform.position.x;
			float cly = cameraTLeft.transform.position.y;
			float crx = cameraTRight.transform.position.x;
			float cry = cameraTRight.transform.position.y;
			for (int i = 0; i < allObjects.Count; i++) {
				float aox = allObjects[i].transform.position.x;
				float aoy = allObjects[i].transform.position.y;
				if( aox + 15 > ctx && aox - 15 < ctx && aoy + 15 > cty && aoy - 15 < cty ||
				  aox + 15 > clx && aox - 15 < clx && aoy + 15 > cly && aoy - 15 < cly ||
				  aox + 15 > crx && aox - 15 < crx && aoy + 15 > cry && aoy - 15 < cry) {}
				else {
					if (moose[allObjects[i].gameObject.GetComponent<HexData>().idNum, exists] == 1) {
						moose[allObjects[i].gameObject.GetComponent<HexData>().idNum, exists] = 0;
						Destroy(allObjects[i].gameObject);
						allObjects.Remove(allObjects[i]);
						i--;
					}
				}
			}
			for ( int i = 0; i < sizeX*sizeY; i++) {
				float aox = moose[i,xCoord];
				float aoy = moose[i,yCoord];
				if( aox + 15 > ctx && aox - 15 < ctx && aoy + 15 > cty && aoy - 15 < cty ||
				   aox + 15 > clx && aox - 15 < clx && aoy + 15 > cly && aoy - 15 < cly ||
				   aox + 15 > crx && aox - 15 < crx && aoy + 15 > cry && aoy - 15 < cry) {
					if (moose[i,exists] == 0) {
						float add = 0f;
						if (moose[i,1] % 2 == 1) { add += 0.57f; }
						Transform hex = Instantiate(tallHex, new Vector3(aox, aoy*1.15f - add, moose[i,elevation]*heightIncrement), Quaternion.identity) as Transform;
						if (moose[i,waterType] > 0 && moose[i,terrainType] != 6) {
							hex.transform.position = new Vector3(hex.transform.position.x, hex.transform.position.y, waterlevel * heightIncrement);
							if (moose[i,waterType] == 2) {
								hex.renderer.material = terrainTypes[7];
							}
							else {
								hex.renderer.material = terrainTypes[0];
							}
						}
						else {
							hex.renderer.material = terrainTypes[moose[i,terrainType]];
						}
						if (moose[i,riverType] > 0) {
							Transform riverHex = Instantiate(categoryPrefab, new Vector3(aox, aoy*1.15f - add, moose[i,elevation]*heightIncrement + 3.4f), Quaternion.identity) as Transform;
							riverHex.renderer.material = riverTypes[moose[i,riverType]];
							riverHex.transform.parent = hex;
							riverHex.transform.Rotate (0,0,-120 + 60*moose[i,riverDirection]);
							riverHex.localScale = new Vector3(1f,1.1f,1f);
						}
						if (moose[i, category] > 1) {
							Transform catHex = Instantiate(categoryPrefab, new Vector3(aox, aoy*1.15f - add, moose[i,elevation]*heightIncrement + 3.45f), Quaternion.identity) as Transform;
							catHex.renderer.material = catTypes[moose[i,category]-2];
							catHex.transform.parent = hex;
							catHex.transform.Rotate (0,0,Random.Range(0, 360));
							catHex.localScale = new Vector3(1f,1.1f,1f);
						} 
						hex.transform.Rotate(0,0,30);
						allObjects.Add (hex);
						hex.gameObject.AddComponent<HexData>();
						hex.gameObject.GetComponent<HexData>().idNum = i;
						moose[i,exists] = 1;
					}
				}
			}
			movementTicker = 0;
		}
	}
}