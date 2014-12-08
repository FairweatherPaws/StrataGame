using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GCScript : MonoBehaviour {
	
	public Transform maaTilkku, miniHex, seaFolder, landFolder, minimapFolder, hexplaneRiver, empty, categoryPrefab, waterfall;
	public Material groundBase, seaDefault, saltSea, underWaterTile, grassTile, tundraTile, aridTile, desertTile, rockyTile, iceWall;
	public Material riverSpawn, riverStraight, riverBendLeft, riverBendRight, riverPond, riverMergeLeft, riverMergeRight, riverMergeSides, riverMergeAll;
	public Material forestMaterial, jungleMaterial, savannahMaterial, marshMaterial, wastelandMaterial;
	public Material mmSea, mmLake, mmIce, mmArid, mmDesert, mmGrass, mmRocky, mmTundra;
	public int sizeX = 30, sizeY = 20, iterations = 3, originalWeight = 6, seaLevel = 70, riverCount = 10, mountainRangeCount = 5, mountainRangeLength = 20;
	public float heightIncrement = 0.05f;
	
	// Use this for initialization
	void Start () {
		int[] miniMultiplier = {sizeX+1, sizeX-1, sizeX, 1, -1, -sizeX, -sizeX-1, -sizeX+1}; // array for scanning nearby hexes, 0-5 for even X coordinates, 2-7 for odd
		int[] multiplier = {2*sizeX+1, 2*sizeX-1, 2*sizeX, sizeX+2, sizeX+1, sizeX, sizeX-1, sizeX-2, 2, 1, -1, -2, -sizeX+2, -sizeX+1, -sizeX, -sizeX-1, -sizeX-2, -2*sizeX, -2*sizeX-1, -2*sizeX+1}; // 2-hex array, even and odd similar to miniMultiplier
		int[] hexSearchOdd = {sizeX, -1, -sizeX-1, -sizeX, -sizeX+1, 1};
		int[] hexSearchEven = {sizeX, sizeX-1, -1, -sizeX, 1, sizeX+1};
		int targetHex = 0;
		int mooseSize = 8;
		int area = sizeX * sizeY;
		Material[] terrainTypes = {underWaterTile, aridTile, desertTile, grassTile, rockyTile, tundraTile};
		Transform[] house = new Transform[area]; // pool of land tiles
		Transform[] ponds = new Transform[area]; // pool of water tiles
		int[,] moose = new int[area,mooseSize]; // land tile data array
		int[,] waterMoose = new int[area,4]; // water tile data array
		int count = 0;
		for (int i = 0; i < area; i++) { // give x and y coordinates to tiles
			for (int j = 0; j < mooseSize; j++) {
				moose[i,j] = 0;
			}
		}
		
		for (int i = 0; i < area; i++) { //generate tiles
			if (i%sizeX == 0) {
				count++;
			}
			moose[i,0] = i;
			moose[i,1] = i - sizeX * (count - 1);
			moose[i,2] = count;
			float add = 0;
			if (moose[i,1] % 2 == 1) { add += 0.57f;}
			Transform uusiHeksa = Instantiate (maaTilkku, new Vector3(moose[i,1], moose[i,2]*1.15f - add, -3.25f), Quaternion.identity) as Transform;
			uusiHeksa.transform.parent = landFolder.transform;
			Transform vesiHeksa = Instantiate (miniHex, new Vector3(moose[i,1], moose[i,2]*1.15f - add, 0), Quaternion.identity) as Transform;
			vesiHeksa.transform.parent = seaFolder.transform;
			uusiHeksa.transform.Rotate(0,0,30);
			vesiHeksa.transform.Rotate (0,0,30);
			vesiHeksa.renderer.material = seaDefault;
			house[i] = uusiHeksa;
			ponds[i] = vesiHeksa;
			HexData Script1 = uusiHeksa.gameObject.AddComponent<HexData>();
			Script1.idNum = i;
			Script1.xLoc = moose[i,1];
			Script1.yLoc = moose[i,2];
			Script1.elev = 0;
			HexData Script2 = vesiHeksa.gameObject.AddComponent<HexData>();
			Script2.idNum = i;
			Script2.xLoc = moose[i,1];
			Script2.yLoc = moose[i,2];
			Script2.elev = 0;
		}
		for (int i = 0; i < area; i++) { // elevation adjustments
			int roll = Random.Range (-100,101);
			moose[i,3] = roll;
			if (moose[i,3] > 70) { moose[i,3] += 10; }
			if (moose[i,3] > 100) { moose[i,3] = 100; }
		}
		for (int i = 0; i < mountainRangeCount; i++) {
			int mountainRangeRoll = Random.Range (2 * area/16, 14*area / 16);
			moose[mountainRangeRoll,3] = 100;
			int bias = Random.Range(0,6);
			int randomDirection = bias;
			int rangeLength = Random.Range (mountainRangeLength / 2, mountainRangeLength * 3 / 2);
			while (rangeLength > 0) {
				if (moose[mountainRangeRoll,1] % 2 == 0) {
					targetHex = hexSearchEven[randomDirection];
					if (mountainRangeRoll+targetHex > 0 && mountainRangeRoll+targetHex < area) {
						moose[mountainRangeRoll+targetHex,3] = 100;
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
						randomDirection = Random.Range (0,6); 
						continue; 
					}
				}
				else {
					targetHex = hexSearchOdd[randomDirection];
					if (mountainRangeRoll+targetHex > 0 && mountainRangeRoll+targetHex < area) {
						moose[mountainRangeRoll+targetHex,3] = 100;
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
						randomDirection = Random.Range (0,6); 
						continue; 
					}
				}
			}
		}
		
		/*
		while (ticker < 4) {
			for (int i = 0; i < area; i++) {
				if (moose[i,3] > 5 || moose[i,3] < -5) {
					if (i > sizeX && i < (area-sizeX)){		//if on b-y row
						if (moose[i,2] > 0 && moose[i,2] < (sizeX-1)) {		//if on b-y column
							// if divisible by 2
							// next row three, previous one
							for (int j = 0; j < 20; j++) {
								int chansu = Random.Range (0,6);
								if (moose[i,1] % 2 == 0) {
									// selection is -30, -1, +1, +29, +30, +31
									if (chansu == 0) {
										moose[i-sizeX,3] = Random.Range (moose[i,3]-1, moose[i,3]+2);
										if (moose[i-sizeX,3] < -10) { moose[i-sizeX,3] = -10; }
										if (moose[i-sizeX,3] > 10) { moose[i-sizeX,3] = 10; }
										break;
									}
									if (chansu == 1) {
									
										moose[i-1,3] = Random.Range (moose[i,3]-1, moose[i,3]+2);
										if (moose[i-1,3] < -10) { moose[i-1,3] = -10; }
										if (moose[i-1,3] > 10) { moose[i-1,3] = 10; }
										break;
									}
									if (chansu == 2) {
										moose[i+1,3] = Random.Range (moose[i,3]-1, moose[i,3]+2);
										if (moose[i+1,3] < -10) { moose[i+1,3] = -10; }
										if (moose[i+1,3] > 10) { moose[i+1,3] = 10; }
										break;
									}
									if (chansu == 3) {
										moose[i+(sizeX-1),3] = Random.Range (moose[i,3]-1, moose[i,3]+2);
										if (moose[i+(sizeX-1),3] < -10) { moose[i+(sizeX-1),3] = -10; }
										if (moose[i+(sizeX-1),3] > 10) { moose[i+(sizeX-1),3] = 10; }
										break;
									}
									if (chansu == 4) {
										moose[i+sizeX,3] = Random.Range (moose[i,3]-1, moose[i,3]+2);
										if (moose[i+sizeX,3] < -10) { moose[i+sizeX,3] = -10; }
										if (moose[i+sizeX,3] > 10) { moose[i+sizeX,3] = 10; }
										break;
									}
									if (chansu == 5) {
										moose[i+(sizeX+1),3] = Random.Range (moose[i,3]-1, moose[i,3]+2);
										if (moose[i+(sizeX+1),3] < -10) { moose[i+(sizeX+1),3] = -10; }
										if (moose[i+(sizeX+1),3] > 10) { moose[i+(sizeX+1),3] = 10; }
										break;
									}

								}
								// else next row one, previous three
								else {
									// selection is -31, -30, -29, -1, +1, +30
									if (chansu == 0) {
										moose[i-(sizeX+1),3] = Random.Range (moose[i,3]-1, moose[i,3]+2);
										if (moose[i-(sizeX+1),3] < -10) { moose[i-(sizeX+1),3] = -10; }
										if (moose[i-(sizeX+1),3] > 10) { moose[i-(sizeX+1),3] = 10; }
										break;
									}
									if (chansu == 1) {
										moose[i-(sizeX),3] = Random.Range (moose[i,3]-1, moose[i,3]+2);
										if (moose[i-(sizeX),3] < -10) { moose[i-(sizeX),3] = -10; }
										if (moose[i-(sizeX),3] > 10) { moose[i-(sizeX),3] = 10; }
										break;
									}
									if (chansu == 2) {
										moose[i-(sizeX-1),3] = Random.Range (moose[i,3]-1, moose[i,3]+2);
										if (moose[i-(sizeX-1),3] < -10) { moose[i-(sizeX-1),3] = -10; }
										if (moose[i-(sizeX-1),3] > 10) { moose[i-(sizeX-1),3] = 10; }
										break;
									}
									if (chansu == 3) {
										moose[i-1,3] = Random.Range (moose[i,3]-1, moose[i,3]+2);
										if (moose[i-1,3] < -10) { moose[i-1,3] = -10; }
										if (moose[i-1,3] > 10) { moose[i-1,3] = 10; }
										break;
									}
									if (chansu == 4) {
										moose[i+1,3] = Random.Range (moose[i,3]-1, moose[i,3]+2);
										if (moose[i+1,3] < -10) { moose[i+1,3] = -10; }
										if (moose[i+1,3] > 10) { moose[i+1,3] = 10; }
										break;
									}
									if (chansu == 5) {
										moose[i+sizeX,3] = Random.Range (moose[i,3]-1, moose[i,3]+2);
										if (moose[i+sizeX,3] < -10) { moose[i+sizeX,3] = -10; }
										if (moose[i+sizeX,3] > 10) { moose[i+sizeX,3] = 10; }
										break;
									}
								}
							}
						}
					}
				} 
			}
			ticker++;
		}
		*/
		for (int j = 0; j < iterations; j++) { // land height smoothing algorithm
			for (int i = 0; i < area; i++) {
				
				int sum = moose[i,3]*originalWeight;
				
				if (moose[i,3] > 70) { // weight on mountains
					sum += 150;
				}
				if (moose[i,3] < -70) { // weight on depths
					sum -= 150;
				}
				
				int duke = originalWeight;
				for (int k = 0; k < 6; k++) {
					if (moose[i,1] % 2 == 0) {
						targetHex = miniMultiplier[k];
						if (i+targetHex > 0 && i+targetHex < area && moose[i,3] != 100 && moose[i,3] != -100) {
							sum += moose[i+targetHex, 3];
							//duke++;
						}
					}
					else {
						targetHex = miniMultiplier[k+2];
						if (i+targetHex > 0 && i+targetHex < area && moose[i,3] != 100 && moose[i,3] != -100) {
							sum += moose[i+targetHex, 3];
							//duke++;
						}
					}
				}
				/*
				if (moose[i,3] != 100 && moose[i,3] != -100) {
					if (moose[i,1] % 2 == 0) {
						// selection is -30, -1, +1, +29, +30, +31
						if (i-(sizeX-1) > 0) {
							sum += moose[i-sizeX, 3];
							duke++;
						}
						if (i > 0) {
							sum += moose[i-1, 3];
							duke++;
						}
						if (i+1 < area) {
							sum += moose[i+1, 3];
							duke++;
						}
						if (i+(sizeX-1) < area) {
							sum += moose[i+(sizeX-1), 3];
							duke++;
						}
						if (i+sizeX < area) {
							sum += moose[i+sizeX, 3];
							duke++;
						}
						if (i+(sizeX+1) < area) {
							sum += moose[i+(sizeX+1), 3];
							duke++;
						}
						
					}
					// else next row one, previous three
					else {
						// selection is -31, -30, -29, -1, +1, +30
						if (i+sizeX < area) {
							sum += moose[i+sizeX, 3];
							duke++;
						}
						if (i+1 < area) {
							sum += moose[i+1, 3];
							duke++;
						}
						if (i > 0) {
							sum += moose[i-1, 3];
							duke++;
						}
						if (i-(sizeX-1) > 0) {
							sum += moose[i-(sizeX-1), 3];
							duke++;
						}
						if (i-sizeX > 0) {
							sum += moose[i-sizeX, 3];
							duke++;
						}
						if (i-(sizeX+1) > 0) {
							sum += moose[i-(sizeX+1), 3];
							duke++;
						}
					}
				}
				*/
				sum /= duke+6;
				HexData Script1 = house[i].GetComponent<HexData>();
				Script1.elev = sum;
				moose[i, 3] = sum;
				if (j == iterations - 1) { 
					house[i].transform.position += new Vector3(0, 0, Script1.elev * heightIncrement);
					house[i].renderer.material = groundBase;
				}
			}
		}
		int jokin = 0; // percent underwater
		int summa = 0;
		
		
		
		for (int i = 0; i < area; i++) { // average height
			summa += moose[i,3];
		}
		summa /= area;
		for ( int j = 0; j < 66; j++) { // sealevel setting alg.
			int checker = 0;
			for (int i = 0; i < area; i++) {
				if (moose[i,3] < summa) {
					checker++;
				}
			}
			jokin = checker * 100 / area;
			if (seaLevel > jokin) {
				summa += 3;
			}
			if (seaLevel < jokin) {
				summa -= 3;
			}
			if (jokin < seaLevel + 3 && jokin > seaLevel - 3) {
				Debug.Log("Bye!");
				break;
				
			}
			Debug.Log("Running!");
			
		}
		int waterTileCount = area;
		for (int i = 0; i < area; i++) { // physical shift of water tiles
			ponds[i].transform.position += new Vector3(0, 0, summa * heightIncrement);
			ponds[i].renderer.material = seaDefault;
			if ( moose[i,3] >= summa ) {
				Destroy(ponds[i].gameObject);
				ponds[i] = null;
				waterTileCount--;
			}
		}
		int seaSize = 1;
		int lakeRange = 18;
		
		
		for (int i = 0; i < area; i++) { // checks to see if sea
			if (ponds[i] != null) {
				int surroundedByWater = 0;
				for (int j = 0; j < lakeRange; j++) {
					if (moose[i,1] % 2 == 0) {
						targetHex = multiplier[j];
						if (i+targetHex > 0 && i+targetHex < area && ponds[i+targetHex] != null) {
							surroundedByWater++;
						}
					}
					else {
						targetHex = multiplier[j+2];
						if (i+targetHex > 0 && i+targetHex < area && ponds[i+targetHex] != null) {
							surroundedByWater++;
						}
					}
				}
				/*
				if (moose[i,1] % 2 == 0) {
					// selection is -30, -1, +1, +29, +30, +31
					if (i-sizeX > 0 && ponds[i-(sizeX)] != null) {
						surroundedByWater++;
					}
					if (i > 0 && ponds[i-1] != null) {
						surroundedByWater++;
					}
					if (i+1 < area && ponds[i+1] != null) {
						surroundedByWater++;
					}
					if (i+(sizeX-1) < area && ponds[i+(sizeX-1)] != null) {
						surroundedByWater++;
					}
					if (i+sizeX < area && ponds[i+sizeX] != null) {
						surroundedByWater++;
					}
					if (i+(sizeX+1) < area && ponds[i+(sizeX+1)] != null) {
						surroundedByWater++;
					}
					
				}
				// else next row one, previous three
				else {
					// selection is -31, -30, -29, -1, +1, +30
					if (i+(sizeX) < area && ponds[i+(sizeX)] != null) {
						surroundedByWater++;
					}
					if (i > 0 && ponds[i-1] != null) {
						surroundedByWater++;
					}
					if (i+1 < area && ponds[i+1] != null) {
						surroundedByWater++;
					}
					if (i-(sizeX-1) > 0 && ponds[i-(sizeX-1)] != null) {
						surroundedByWater++;
					}
					if (i-sizeX > 0 && ponds[i-sizeX] != null) {
						surroundedByWater++;
					}
					if (i-(sizeX+1) > 0 && ponds[i-(sizeX+1)] != null) {
						surroundedByWater++;
					}
					
				}
				*/
				
				if (surroundedByWater > lakeRange * 5 / 6) {
					int whatWhat = Random.Range(0, 5);
					if (whatWhat > 2) {waterMoose[i,3] = 1;}
				}
			}
		}
		for (int j = 0; j < sizeX * 2; j++) { // spreads seas
			for (int i = 0; i < area; i++) {
				if (ponds[i] != null) {
					for (int k = 0; k < 6; k++) {
						if (moose[i,1] % 2 == 0) {
							targetHex = miniMultiplier[k];
							if (i+targetHex > 0 && i+targetHex < area && ponds[i+targetHex] != null && waterMoose[i+targetHex, 3] == 0 && waterMoose[i,3] == 1) {
								seaSize++;
								waterMoose[i+targetHex, 3] = waterMoose[i,3];
							}
						}
						else {
							targetHex = miniMultiplier[k+2];
							if (i+targetHex > 0 && i+targetHex < area && ponds[i+targetHex] != null && waterMoose[i+targetHex, 3] == 0 && waterMoose[i,3] == 1) {
								seaSize++;
								waterMoose[i+targetHex, 3] = waterMoose[i,3];
							}
						}
					}
					/*
					if (moose[i,1] % 2 == 0) {
						// selection is -30, -1, +1, +29, +30, +31
						if (i-sizeX > 0 && ponds[i-(sizeX)] != null && waterMoose[i-(sizeX), 3] == 0 && waterMoose[i,3] == 1) {
							waterMoose[i-(sizeX), 3] = waterMoose[i,3];
							seaSize++;
						}
						if (i > 0 && ponds[i-1] != null && waterMoose[i-1, 3] == 0 && waterMoose[i,3] == 1) {
							waterMoose[i-1, 3] = waterMoose[i,3];
							seaSize++;
						}
						if (i+1 < area && ponds[i+1] != null && waterMoose[i+1, 3] == 0 && waterMoose[i,3] == 1) {
							waterMoose[i+1, 3] = waterMoose[i,3];
							seaSize++;
						}
						if (i+(sizeX-1) < area && ponds[i+(sizeX-1)] != null && waterMoose[i+(sizeX-1), 3] == 0 && waterMoose[i,3] == 1) {
							waterMoose[i+(sizeX-1), 3] = waterMoose[i,3];
							seaSize++;
						}
						if (i+sizeX < area && ponds[i+sizeX] != null && waterMoose[i+sizeX, 3] == 0 && waterMoose[i,3] == 1) {
							waterMoose[i+sizeX, 3] = waterMoose[i,3];
							seaSize++;
						}
						if (i+(sizeX+1) < area && ponds[i+(sizeX+1)] != null && waterMoose[i+(sizeX+1), 3] == 0 && waterMoose[i,3] == 1) {
							waterMoose[i+(sizeX+1), 3] = waterMoose[i,3];
							seaSize++;
						}
						
					}
					// else next row one, previous three
					else {
						// selection is -31, -30, -29, -1, +1, +30
						if (i+(sizeX) < area && ponds[i+(sizeX)] != null && waterMoose[i+(sizeX), 3] == 0 && waterMoose[i,3] == 1) {
							waterMoose[i+(sizeX), 3] = waterMoose[i,3];
							seaSize++;
						}
						if (i > 0 && ponds[i-1] != null && waterMoose[i-1, 3] == 0 && waterMoose[i,3] == 1) {
							waterMoose[i-1, 3] = waterMoose[i,3];
							seaSize++;
						}
						if (i+1 < area && ponds[i+1] != null && waterMoose[i+1, 3] == 0 && waterMoose[i,3] == 1) {
							waterMoose[i+1, 3] = waterMoose[i,3];
							seaSize++;
						}
						if (i-(sizeX-1) > 0 && ponds[i-(sizeX-1)] != null && waterMoose[i-(sizeX-1), 3] == 0 && waterMoose[i,3] == 1) {
							waterMoose[i-(sizeX-1), 3] = waterMoose[i,3];
							seaSize++;
						}
						if (i-sizeX > 0 && ponds[i-sizeX] != null && waterMoose[i-sizeX, 3] == 0 && waterMoose[i,3] == 1) {
							waterMoose[i-sizeX, 3] = waterMoose[i,3];
							seaSize++;
						}
						if (i-(sizeX+1) > 0 && ponds[i-(sizeX+1)] != null && waterMoose[i-(sizeX+1), 3] == 0 && waterMoose[i,3] == 1) {
							waterMoose[i-(sizeX+1), 3] = waterMoose[i,3];
							seaSize++;
						}

					}
					*/
				}
			}
		}
		
		for (int i = 0; i < area; i++) { // changes sea textures
			if (ponds[i] != null) {
				HexData Script1 = ponds[i].gameObject.GetComponent<HexData>();
				Script1.elev = waterMoose[i,3];
				if (waterMoose[i,3] == 1) {
					ponds[i].renderer.material = saltSea;
				}
			}
		}
		Debug.Log (summa); // sealevel value
		
		for (int i = 0; i < area; i++) { // next up, temperature bands and elevation effects
			
			// random allocation of base tiles everywhere
			// 0 	-1, 
			// 1 	 2, 
			// 2 	-3, 
			// 3 	 5,
			// 4 	-2,
			// 5 	 1}
			int chulchul = Random.Range (0, 20);
			if (chulchul < 5) { 
				moose[i,4] = 1; 
				moose[i,5] = 4;
			}
			else if (chulchul > 18) { 
				moose[i,4] = 2; 
				moose[i,5] = -6;
			}
			else { 
				moose[i,4] = 3; 
				moose[i,5] = 10;
			}
			
			if (moose[i,2] < sizeY * 2 / 16 || moose[i,2] > sizeY * 14 / 16) { // tundra near poles
				moose[i,4] = 5;
				moose[i,5] = 2;
			}
			if (moose[i,2] < sizeY * 9 / 16 && moose[i,2] > sizeY * 7 / 16) { // equatorial
				int chulchula = Random.Range (0,20);
				if (chulchula < 5) { 
					moose[i,4] = 1; 
					moose[i,5] = 4;
				}
				else if (chulchula > 15) { 
					moose[i,4] = 3; 
					moose[i,5] = 10;
				}
				else { 
					moose[i,4] = 2; 
					moose[i,5] = -6;
				}
			}
			if (moose[i,3] > 45) { 
				moose[i,4] = 4; 
				moose[i,5] = -4;
			} // rocky type for peaks
			house[i].renderer.material = terrainTypes[moose[i,4]];
		}
		for (int j = 0; j < iterations; j++) { // terrain smoother
			for (int i = 0; i < area; i++) {
				int which = Random.Range(-3,6);
				if (which > -1) { 
					for (int k = 0; k < 6; k++) {
						if (k == which) {
							if (moose[i,1] % 2 == 0) {
								targetHex = miniMultiplier[k];
								if (i+targetHex > 0 && i+targetHex < area) {
									if (moose[i+targetHex,4] > 0 && moose[i+targetHex,3] < 46) { moose[i,4] = moose[i+targetHex,4];}
								}
							}
							else {
								targetHex = miniMultiplier[k+2];
								if (i+targetHex > 0 && i+targetHex < area) {
									if (moose[i+targetHex,4] > 0 && moose[i+targetHex,3] < 46) { moose[i,4] = moose[i+targetHex,4];}
									
								}
							}
						}
					}
				}
				if (moose[i,3] > 45) { moose[i,4] = 4; } // rocky type for peaks
				if (moose[i,3] < summa) { // underwater terrain
					moose[i,4] = 0;
					if (waterMoose[i,3] == 1) {	
						moose[i,5] = 0; 
					}
					else { 
						moose[i,5] = 6; 
					}
				}
				if (moose[i,4] == 1) {
					moose[i,5] = 4;
				}
				if (moose[i,4] == 2) {
					moose[i,5] = -6;
				}
				if (moose[i,4] == 3) {
					moose[i,5] = 10;
				}
				if (moose[i,4] == 4) {
					moose[i,5] = -4;
				}
				if (moose[i,4] == 5) {
					moose[i,5] = 2;
				}
				
				house[i].renderer.material = terrainTypes[moose[i,4]];
				house[i].GetComponent<HexData>().terrainType = moose[i,4];
			}
		}
		int[] fertilityArray = new int[area];
		for (int i = 0; i < area; i++) {// fertility settings
			fertilityArray[i] = moose[i,5];
			for (int k = 0; k < 6; k++) {
				if (moose[i,1] % 2 == 0) {
					targetHex = miniMultiplier[k];
					if (i+targetHex > 0 && i+targetHex < area) {
						fertilityArray[i] += moose[i+targetHex,5] / 2;
					}
				}
				else {
					targetHex = miniMultiplier[k+2];
					if (i+targetHex > 0 && i+targetHex < area) {
						fertilityArray[i] += moose[i+targetHex,5] / 2;
					}
				}
			}
		}
		for (int i = 0; i < area; i++) {
			moose[i,5] += fertilityArray[i];
			HexData Script1 = house[i].GetComponent<HexData>();
			Script1.fertility = moose[i,5];
		}
		int riverTileCount = 0;
		int[,] riverStart = new int[riverCount,2];
		Transform[] riverSpawns = new Transform[riverCount];
		List<Transform> allRivers = new List<Transform>();
		for (int i = 0; i < area; i++) {
			int lowest = riverStart[0,1];
			int lowestId = 0;
			int lowestNum = riverStart[0,0];
			for (int j = 0; j < riverCount; j++) {
				if (riverStart[j,1] < lowest) {
					lowest = riverStart[j,1];
					lowestNum = riverStart[j,0];
					lowestId = j;
				}
			}
			
			
			if (moose[i,3] > riverStart[lowestId,1]) {
				bool notThere = true;
				for (int j = 0; j < 6; j++) {
					for (int k = 0; k < riverCount; k++) {
						if (moose[i,1] % 2 == 0) {
							targetHex = miniMultiplier[j];
							if (i+targetHex > 0 && i+targetHex < area) {
								if (riverStart[k,0] == i+targetHex) { 
									notThere = false;
								}
							}
						}
						else {
							targetHex = miniMultiplier[j+2];
							if (i+targetHex > 0 && i+targetHex < area) {
								if (riverStart[k,0] == i+targetHex) { 
									notThere = false;
								}
							}
						}
					}
				}
				if (notThere) {
					riverStart[lowestId, 0] = i;
					riverStart[lowestId, 1] = moose[i,3];
				}
			}
		}
		for (int i = 0; i < riverCount; i++) {
			float add = 0f;
			if (moose[riverStart[i,0],1] % 2 == 1) { add += 0.57f;}
			Transform joki = Instantiate (hexplaneRiver, new Vector3(moose[riverStart[i,0],1], moose[riverStart[i,0],2]*1.15f - add, riverStart[i,1] * heightIncrement + 0.15f), Quaternion.identity) as Transform;
			joki.gameObject.AddComponent<RiverData>();
			joki.localScale = new Vector3(1f,1.1f,1f);
			joki.transform.Rotate(0,0,30);
			riverTileCount++;
			riverSpawns[i] = joki;
			allRivers.Add (joki);
			joki.renderer.material = riverSpawn;
			joki.GetComponent<RiverData>().type = "River Start";
			RiverData Script1 = joki.GetComponent<RiverData>();
			Script1.id = riverStart[i,0];
			Script1.elev = riverStart[i,1];
			Script1.prev = riverStart[i,0];
			moose[riverStart[i,0],6] = 1;
			int direction = 0;
			int lowest = riverStart[i,1];
			int target = riverStart[i,0];
			for (int j = 0; j < 6; j++) {
				if (moose[riverStart[i,0],1] % 2 == 0) {
					targetHex = hexSearchEven[j];
					if (riverStart[i,0]+targetHex > 0 && riverStart[i,0]+targetHex < area) {
						if (moose[riverStart[i,0]+targetHex,3] < lowest) { 
							lowest = moose[riverStart[i,0]+targetHex,3];
							target = moose[riverStart[i,0]+targetHex,0];
							direction = j;
						}
					}
				}
				else {
					targetHex = hexSearchOdd[j];
					if (riverStart[i,0]+targetHex > 0 && riverStart[i,0]+targetHex < area) {
						if (moose[riverStart[i,0]+targetHex,3] < lowest) { 
							lowest = moose[riverStart[i,0]+targetHex,3];
							target = moose[riverStart[i,0]+targetHex,0];
							direction = j;
						}
					}
				}
			}
			Script1.newDir = direction;
		}
		for (int i = 0; i < riverCount; i++) {
			List<Transform> spawnedRivers = new List<Transform>();
			spawnedRivers.Add(riverSpawns[i]);
			Transform River = Instantiate (empty, new Vector3(0,0,0), Quaternion.identity) as Transform;
			River.name = "River " + i;
			riverSpawns[i].parent = River;
			bool running = true;
			int countise = 0;
			int oldDirection = 0;
			int direction = 0;
			
			while (running) {
				int lowest = riverStart[i,1];
				int target = riverStart[i,0];
				if (countise == 0) {
					oldDirection = riverSpawns[i].GetComponent<RiverData>().newDir;
				}
				else {
					oldDirection = direction;
				}
				direction = 0;
				for (int j = 0; j < 6; j++) {
					if (moose[riverStart[i,0],1] % 2 == 0) {
						targetHex = hexSearchEven[j];
						if (riverStart[i,0]+targetHex > 0 && riverStart[i,0]+targetHex < area) {
							if (moose[riverStart[i,0]+targetHex,3] < lowest) { 
								lowest = moose[riverStart[i,0]+targetHex,3];
								target = moose[riverStart[i,0]+targetHex,0];
								direction = j;
							}
						}
					}
					else {
						targetHex = hexSearchOdd[j];
						if (riverStart[i,0]+targetHex > 0 && riverStart[i,0]+targetHex < area) {
							if (moose[riverStart[i,0]+targetHex,3] < lowest) { 
								lowest = moose[riverStart[i,0]+targetHex,3];
								target = moose[riverStart[i,0]+targetHex,0];
								direction = j;
							}
						}
					}
				}
				bool merge = false;
				bool waterfall = false;
				if (lowest + 15 < riverStart[i,1]) {
					waterfall = true;
				}
				// Debug.Log("ID: " + target + " " + direction + " " + oldDirection);
				if (target != riverStart[i,0]) {
					RiverData Script2 = spawnedRivers[countise].GetComponent<RiverData>();
					Script2.next = target;
					
					if (countise == 0) {
						//spawnedRivers[countise].transform.Rotate(0,direction*60,0);
					}
					if (oldDirection == -1) {
						oldDirection = 5;
					}
					if (oldDirection == 5 && direction == 0) { 
						oldDirection = -1;
					}
					if (oldDirection == 0 && direction == 5) {
						direction = -1;
					}
					if (oldDirection > direction && countise > 0) {
						spawnedRivers[countise].renderer.material = riverBendLeft;
						spawnedRivers[countise].GetComponent<RiverData>().type = "Bend Left";
						//spawnedRivers[countise].transform.Rotate(0,240+direction*60,0);
						
					}
					if (oldDirection < direction && countise > 0) {
						spawnedRivers[countise].renderer.material = riverBendRight;
						spawnedRivers[countise].GetComponent<RiverData>().type = "Bend Right";
						//spawnedRivers[countise].transform.Rotate(0,120+direction*60,0);
						
					}
					if (oldDirection == direction && countise > 0) {
						spawnedRivers[countise].renderer.material = riverStraight;
						spawnedRivers[countise].GetComponent<RiverData>().type = "Straight";
						//spawnedRivers[countise].transform.Rotate(0,direction*60,0);
					}
					countise++;
					if (moose[target, 3] < summa) {
						Script2.terminate = 2;
						spawnedRivers[countise-1].GetComponent<RiverData>().oldDir = oldDirection;
						//						if (oldDirection < 3) { spawnedRivers[countise-1].GetComponent<RiverData>().oldDir = oldDirection + 3;}
						//						if (oldDirection > 2) { spawnedRivers[countise-1].GetComponent<RiverData>().oldDir = oldDirection - 3;}
						spawnedRivers[countise-1].GetComponent<RiverData>().newDir = direction;
						break;
					}
					if (moose[target, 6] == 1) {
						Script2.terminate = 1;
						spawnedRivers[countise-1].GetComponent<RiverData>().oldDir = oldDirection;
						//						if (oldDirection < 3) { spawnedRivers[countise-1].GetComponent<RiverData>().oldDir = oldDirection + 3;}
						//						if (oldDirection > 2) { spawnedRivers[countise-1].GetComponent<RiverData>().oldDir = oldDirection - 3;}
						spawnedRivers[countise-1].GetComponent<RiverData>().newDir = direction;
						merge = true;
						
					}
					
					if (merge) { 
						for (int p = 0; p < allRivers.Count; p++) {
							if (allRivers[p].GetComponent<RiverData>().id == target) {
								int told = allRivers[p].GetComponent<RiverData>().oldDir;
								int mnew = spawnedRivers[countise-1].GetComponent<RiverData>().newDir;
								if (allRivers[p].GetComponent<RiverData>().type == "Straight") {
									if (told == mnew + 1) {
										allRivers[p].renderer.material = riverMergeRight;
										//allRivers[p].transform.Rotate (0,180,0);
									}
									if (told == mnew + 5 || told == mnew - 1) {
										allRivers[p].renderer.material = riverMergeLeft;
										//allRivers[p].transform.Rotate (0,180,0);
									}
									
									if (allRivers[p].GetComponent<RiverData>().incoming == 2) {
										allRivers[p].renderer.material = riverMergeAll;
									}
								}
								if (allRivers[p].GetComponent<RiverData>().type == "Bend Right") {
									if (told == mnew - 1) {
										allRivers[p].renderer.material = riverMergeRight;
										//allRivers[p].transform.Rotate (0,60,0);
									}
									if (told == mnew - 2) {
										allRivers[p].renderer.material = riverMergeSides;
									}
									if (allRivers[p].GetComponent<RiverData>().incoming == 2) {
										allRivers[p].renderer.material = riverMergeAll;
										//allRivers[p].transform.Rotate (0,0,0);
									}
								}
								if (allRivers[p].GetComponent<RiverData>().type == "Bend Left") {
									if (told == mnew + 1) {
										allRivers[p].renderer.material = riverMergeLeft;
										//allRivers[p].transform.Rotate (0,300,0);
									}
									if (told == mnew + 2) {
										allRivers[p].renderer.material = riverMergeSides;
									}
									if (allRivers[p].GetComponent<RiverData>().incoming == 2) {
										allRivers[p].renderer.material = riverMergeAll;
										//allRivers[p].transform.Rotate (0,0,0);
									}
								}
								
								allRivers[p].GetComponent<RiverData>().incoming++;
							}
						}
						break; 
					}
					float add = 0f;
					
					if (moose[target,1] % 2 == 1) { add += 0.57f; }
					Transform joki = Instantiate (hexplaneRiver, new Vector3(moose[target,1], moose[target,2]*1.15f - add, lowest * heightIncrement + 0.15f), Quaternion.identity) as Transform;
					joki.gameObject.AddComponent<RiverData>();
					//Transform vesiputous = Instantiate(empty, new Vector3((moose[target,1] + moose[riverStart[i,0],1])*0.5f, (moose[target,2] + moose[riverStart[i,0],2])*0.5f*1.15f - add*0.5f, riverStart[i,1] * heightIncrement + 0.12f), Quaternion.identity) as Transform;
					joki.transform.Rotate(0,0,30);
					//vesiputous.gameObject.GetComponent<ParticleSystem>().renderer.enabled = true;
					joki.localScale = new Vector3(1f,1.1f,1f);
					allRivers.Add (joki);
					riverTileCount++;
					RiverData Script1 = joki.GetComponent<RiverData>();
					Script1.incoming = 1;
					Script1.id = target;
					Script1.elev = lowest;
					Script1.prev = riverStart[i,0];
					//					if (oldDirection < 3) { spawnedRivers[countise-1].GetComponent<RiverData>().oldDir = oldDirection + 3;}
					//					if (oldDirection > 2) { spawnedRivers[countise-1].GetComponent<RiverData>().oldDir = oldDirection - 3;}
					spawnedRivers[countise-1].GetComponent<RiverData>().oldDir = oldDirection;
					spawnedRivers[countise-1].GetComponent<RiverData>().newDir = direction;
					joki.parent = River;
					spawnedRivers.Add(joki);
					moose[target,6] = 1;
					riverStart[i,0] = target;
					riverStart[i,1] = lowest;
					
					
				}
				else {
					spawnedRivers[countise].renderer.material = riverPond;
					spawnedRivers[countise].GetComponent<RiverData>().type = "End Pond";
					spawnedRivers[countise].GetComponent<RiverData>().terminate = 3;
					spawnedRivers[countise].GetComponent<RiverData>().oldDir = oldDirection;
					//					if (oldDirection < 3) { spawnedRivers[countise].GetComponent<RiverData>().oldDir = oldDirection + 3;}
					//					if (oldDirection > 2) { spawnedRivers[countise].GetComponent<RiverData>().oldDir = oldDirection - 3;}
					//spawnedRivers[countise].transform.Rotate(0,180 + oldDirection*60,0);
					running = false;
				}
			}
			spawnedRivers[0].GetComponent<RiverData>().oldDir = 0;
		}
		
		List<Transform> allForests = new List<Transform>();
		Transform forestFolder = Instantiate (empty, new Vector3(0,0,0), Quaternion.identity) as Transform;
		forestFolder.name = "forestFolder";
		
		List<Transform> allJungles = new List<Transform>();
		Transform jungleFolder = Instantiate (empty, new Vector3(0,0,0), Quaternion.identity) as Transform;
		jungleFolder.name = "jungleFolder";
		
		List<Transform> allSavannahs = new List<Transform>();
		Transform savannahFolder = Instantiate (empty, new Vector3(0,0,0), Quaternion.identity) as Transform;
		savannahFolder.name = "savannahFolder";
		
		List<Transform> allIce = new List<Transform>();
		Transform iceFolder = Instantiate (empty, new Vector3(0,0,0), Quaternion.identity) as Transform;
		iceFolder.name = "iceFolder";
		
		List<Transform> allMarshes = new List<Transform>();
		Transform marshFolder = Instantiate (empty, new Vector3(0,0,0), Quaternion.identity) as Transform;
		marshFolder.name = "marshFolder";
		
		List<Transform> allWastelands = new List<Transform>();
		Transform wastelandFolder = Instantiate (empty, new Vector3(0,0,0), Quaternion.identity) as Transform;
		wastelandFolder.name = "wastelandFolder";
		
		foreach (Transform riverTile in allRivers) {
			if (riverTile.gameObject.GetComponent<RiverData>().type == "End Pond") {
				int roller = Random.Range (0, 20);
				int i = riverTile.gameObject.GetComponent<RiverData>().id;
				if (roller < 4 && moose[i,2] < sizeY * 13 / 16 && moose[i,2] > sizeY * 3 / 16) {
					moose[i, 7] = 5;
					moose[i, 5] -= 20;
					house[i].gameObject.GetComponent<HexData>().fertility -= 20;
					house[i].gameObject.GetComponent<HexData>().category = 5;
					float add = 0f;
					if (moose[i,1] % 2 == 1) { add += 0.57f; }
					Transform suo = Instantiate (categoryPrefab, new Vector3(moose[i,1], moose[i,2]*1.15f - add, moose[i,3] * heightIncrement + 0.19f), Quaternion.identity) as Transform;
					suo.renderer.material = marshMaterial;
					suo.transform.Rotate(0,0,Random.Range (0, 360));
					suo.parent = marshFolder;
					suo.name = "marsh tile";
					allMarshes.Add(suo);
				}
			}
		}
		
		for (int i = 0; i < area; i++) {
			if (moose[i,6] == 1) {
				house[i].GetComponent<HexData>().fertility += 20;
				moose[i,5] += 20;
				house[i].GetComponent<HexData>().hasRiver = 1;
			}
			if (moose[i,2] > sizeY * 15 / 16 || moose[i,2] < sizeY / 16 + 2) {
				if (moose[i,3] < 40) {
					moose[i,3] = 40;
					moose[i,4] = 6;
					moose[i,7] = 1;
					house[i].transform.position += new Vector3(0, 0, (40 - house[i].GetComponent<HexData>().elev) * heightIncrement);
					house[i].GetComponent<HexData>().elev = 40;
					house[i].GetComponent<HexData>().category = 1;
					house[i].renderer.material = iceWall;
					allIce.Add(house[i]);
				}
			}
			if (3 > Random.Range (0,100) && moose[i,4] != 4 && moose[i,4] != 2 && moose[i,7] == 0 && moose[i,4] != 0) {
				moose[i,7] = 6;
				house[i].GetComponent<HexData>().category = 6;
				moose[i,5] -= 40;
				house[i].GetComponent<HexData>().fertility -= 40;
				float add = 0f;
				if (moose[i,1] % 2 == 1) { add += 0.57f; }
				Transform joutomaa = Instantiate (categoryPrefab, new Vector3(moose[i,1], moose[i,2]*1.15f - add, moose[i,3] * heightIncrement + 0.19f), Quaternion.identity) as Transform;
				joutomaa.renderer.material = wastelandMaterial;
				joutomaa.transform.Rotate(0,0,Random.Range (0, 360));
				joutomaa.parent = wastelandFolder;
				joutomaa.name = "wasteland tile";
				allWastelands.Add(joutomaa);
			}
			
			if (moose[i,5] > 40 && moose[i,4] == 3 && moose[i,7] == 0) {
				if (moose[i,2] > sizeY * 6 / 16 && moose[i,2] < sizeY * 10 / 16) {
					float add = 0f;
					if (moose[i,1] % 2 == 1) { add += 0.57f; }
					Transform viidakko = Instantiate (categoryPrefab, new Vector3(moose[i,1], moose[i,2]*1.15f - add, moose[i,3] * heightIncrement + 0.19f), Quaternion.identity) as Transform;
					viidakko.renderer.material = jungleMaterial;
					viidakko.transform.Rotate(0,0,Random.Range (0, 360));
					viidakko.parent = jungleFolder;
					viidakko.name = "jungle tile";
					allJungles.Add(viidakko);
					moose[i,7] = 3;
					house[i].GetComponent<HexData>().fertility += 20;
					house[i].GetComponent<HexData>().category = 3;
				}
				else {
					float add = 0f;
					if (moose[i,1] % 2 == 1) { add += 0.57f; }
					Transform metsä = Instantiate (categoryPrefab, new Vector3(moose[i,1], moose[i,2]*1.15f - add, moose[i,3] * heightIncrement + 0.19f), Quaternion.identity) as Transform;
					metsä.renderer.material = forestMaterial;
					metsä.transform.Rotate(0,0,Random.Range (0, 360));
					metsä.parent = forestFolder;
					metsä.name = "forest tile";
					allForests.Add(metsä);
					moose[i,7] = 2;
					house[i].GetComponent<HexData>().fertility += 10;
					house[i].GetComponent<HexData>().category = 2;
				}
			}
			if (moose[i,5] > 20 && moose[i,4] == 1 && moose[i,7] == 0) {
				float add = 0f;
				if (moose[i,1] % 2 == 1) { add += 0.57f; }
				Transform savanni = Instantiate (categoryPrefab, new Vector3(moose[i,1], moose[i,2]*1.15f - add, moose[i,3] * heightIncrement + 0.19f), Quaternion.identity) as Transform;
				savanni.renderer.material = savannahMaterial;
				savanni.transform.Rotate(0,0,Random.Range (0, 360));
				savanni.parent = savannahFolder;
				savanni.name = "savannah tile";
				allSavannahs.Add(savanni);
				moose[i,7] = 4;
				house[i].GetComponent<HexData>().fertility += 10;
				house[i].GetComponent<HexData>().category = 4;
			}
		}
		for (int i = 0; i < area; i++) {
			float add = 0f;
			if (moose[i,1] % 2 == 1) { add += 0.57f;}
			Transform minimap = Instantiate (hexplaneRiver, new Vector3(moose[i,1], moose[riverStart[i,2]*1.15f - add - 100, 0), Quaternion.identity) as Transform;
			                                                                              minimap.localScale = new Vector3(1f,1.1f,1f);
			                                                                              minimap.transform.Rotate(0,0,30);
			                                                                              }
			                                                                              }
			                                                                              
			                                                                              // Update is called once per frame
			                                                                              void Update () {
				
			                                                                              }
			                                                                              }
			                                                                              
			                                                                              // moose[tileid, x]
			                                                                              // x = 0: tileid
			                                                                              // x = 1: tile x-coord
			                                                                              // x = 2: tile y-coord
			                                                                              // x = 3: tile elevation
			                                                                              // x = 4: tile terrain type {0: underWaterTile 0/3, 1: aridTile 2, 2: desertTile -3, 3: grassTile 5, 4: rockyTile -1, 5: tundraTile 1, 6: ICE --}
			                                                                              // x = 5: tile fertility value
			                                                                              // x = 6: has river?
			                                                                              // x = 7: category {0: nothing 1: ICE 2:forest, 3:jungle, 4:savannah, 5: marsh, 6: wasteland