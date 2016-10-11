kernel void findIntersection(global read_only float* data, int dataSize, int startRender, int endRender, int screenWidth, int screenHeight, int aaDepth) {

}
	//sceneObjects format
	/*
		position
		0 - x
		1 - y
		2 - z

		color
		3 - r
		4 - g
		5 - b
		6 - special

		type
		7 - type

		normal
		8 - x
		9 - y
		10 - z

		additional geometry data, sphere: center
		11 - x
		12 - y
		13 - z
		14 - sphere: radius, plane: distance

		

	*/

float findIntersectionPlane(float* sceneObjects, int objectIndex, float* ray, float distance){
	float rayOriginX = ray[0];
	float rayOriginY = ray[1];
	float rayOriginZ = ray[2];

	float rayDirectionX = ray[4];
	float rayDirectionY = ray[5];
	float rayDirectionZ = ray[6];

	float normalX = sceneObjects[objectIndex+8];
	float normalY = sceneObjects[objectIndex+9];
	float normalZ = sceneObjects[objectIndex+10];

	float a = rayDirectionX * normalX + rayDirectionY * normalY + rayDirectionZ * normalZ;

	if(a == 0)
	{
		return -1;
	}else
	{
		float rX = rayOriginX + (normalX * distance * -1);
		float rY = rayOriginY + (normalY * distance * -1);
		float rZ = rayOriginZ + (normalZ * distance * -1);


		float b = rX * normalX + rY * normalY + rZ * normalZ;
		return -1*b/a;
	}
}

float findIntersectionSphere(float* sceneObjects, int objectIndex, float* ray, float radius){
	float rayOriginX = ray[0];
	float rayOriginY = ray[1];
	float rayOriginZ = ray[2];

	float rayDirectionX = ray[3];
	float rayDirectionY = ray[4];
	float rayDirectionZ = ray[5];

	float sphereCenterX = sceneObjects[objectIndex + 11];
	float sphereCenterY = sceneObjects[objectIndex + 12];
	float sphereCenterZ = sceneObjects[objectIndex + 13];

	float a = 1;
	float b = (2 * (rayOriginX - sphereCenterX) * rayDirectionX) + (2 * (rayOriginY - sphereCenterY) * rayDirectionY) + (2 * (rayOriginZ - sphereCenterZ) * rayDirectionZ);
	float c = pow(rayOriginX - sphereCenterX, 2) + pow(rayOriginY - sphereCenterY, 2) + pow(rayOriginZ - sphereCenterZ, 2) - pow(radius,2);

	float discriminant = b * b - 4 * c;

	if(discriminant > 0)
	{
		float rootOne = ((-1 * b - sqrt(discriminant)) / 2) - 0.000001;

		if(rootOne > 0)
		{
			return rootOne;
		}else
		{
			return ((sqrt(discriminant) - b) / 2) - 0.000001;
		}
	}else
	{
		return -1;
	}
}

int winningObjectIndex(float* intersections, int intersectionsSize)
{
    //returns the index of winning intersection
    int minimumIndex = -1;

    //prevent useless calculations
    if(intersectionsSize == 0)
    {
        return -1;
    }else if (intersectionsSize == 1)
    {
        if (intersections[0] > 0)
        {
            //if that intersection is greater than zero then its first intersection
            return 0;
        }else
        {
            //otherwise the only intersection value is negative, missed
            return -1;
        }
    }else
    {
        //otherwise there is more than one and we have to find the max
        float maxValue = 0;
        for (int index = 0; index < intersectionsSize; index++)
        {
            if (maxValue < intersections[index])
            {
                maxValue = intersections[index];
            }
        }
        //then starting from max, find the min positive value
        if(maxValue > 0)
        {
            //we only want positive intersections
            for (int index = 0; index < intersectionsSize; index++)
            {
                if(intersections[index] > 0 && intersections[index] <= maxValue)
                {
                    maxValue = intersections[index];
                    minimumIndex = index;
                }
            }
            return minimumIndex;
        }else
        {
            //all intersections negative
            return -1;
        }
    }
}


float4 getColorAt(float3 intersectionPosition, float3 intersectionRayDirection, float* sceneObjects, int sceneObjectsSize, float* lightSources, int lightSourcesSize, int winnerIndex, float accurary, float ambientlight, int bounces)
{
	//sceneObjects format
	/*
		position
		0 - x
		1 - y
		2 - z

		color
		3 - r
		4 - g
		5 - b
		6 - special

		type
		7 - type

		normal
		8 - x
		9 - y
		10 - z

		additional geometry data, sphere: center
		11 - x
		12 - y
		13 - z
		14 - sphere: radius, plane: distance

		

	*/

	//lightSources format
	/*
		position
		0 - x
		1 - y
		2 - z

		color
		3 - r
		4 - g
		5 - b
		6 - special
	*/

	int winnerType = sceneObjects[winnerIndex+6];

    float4 winningObjectColor;
	winningObjectColor[0] = sceneObjects[winnerIndex+3];
	winningObjectColor[1] = sceneObjects[winnerIndex+4];
	winningObjectColor[2] = sceneObjects[winnerIndex+5];
	winningObjectColor[3] = sceneObjects[winnerIndex+6];


	float3 winningObjectNormal[3];
	if(objectType == 0){
		//plane
		winningObjectNormal[0] = sceneObjects[winnerIndex+8];
		winningObjectNormal[1] = sceneObjects[winnerIndex+9];
		winningObjectNormal[2] = sceneObjects[winnerIndex+10];
	}

	if(objectType == 1){
		//sphere
		winningObjectNormal[0] = (intersectionPosition[0] + (sceneObjects[winnerIndex+11] * -1));
		winningObjectNormal[1] = (intersectionPosition[1] + (sceneObjects[winnerIndex+12] * -1));
		winningObjectNormal[2] = (intersectionPosition[2] + (sceneObjects[winnerIndex+13] * -1));
		//normalize
		float magnitude = sqrt((winningObjectNormal[0]*winningObjectNormal[0]) + (winningObjectNormal[1]*winningObjectNormal[1]) + (winningObjectNormal[2]*winningObjectNormal[2]));
		winningObjectNormal[0] = winningObjectNormal[0]/magnitude;
		winningObjectNormal[1] = winningObjectNormal[1]/magnitude;
		winningObjectNormal[2] = winningObjectNormal[2]/magnitude;
	}

    float4 finalColor;
	finalColor[0] = winningObjectColor[0] * ambientlight;
	finalColor[1] = winningObjectColor[1] * ambientlight;
	finalColor[2] = winningObjectColor[2] * ambientlight;
	finalColor[3] = winningObjectColor[3] * ambientlight;

    if(winningObjectColor[3] == 2)
    {
        int square = (int)floor(intersectionPosition[0]) + (int)floor(intersectionPosition[2]);

        if((square % 2) == 0)
        {
            winningObjectColor[0] = 0;
            winningObjectColor[1] = 0;
            winningObjectColor[2] = 0;
        }
        else
        {
            winningObjectColor[0] = 1;
            winningObjectColor[1] = 1;
            winningObjectColor[2] = 1;
        }
    }

    if(winningObjectColor[3] > 0 && winningObjectColor[3] <= 1)
    {
        //reflection from objects that have specular value
        float dot1 = simple_dot(winningObjectNormal, negate(intersectionRayDirection));
        float3 scalar1;
		scalar1[0] = winningObjectNormal[0] *dot1;
		scalar1[1] = winningObjectNormal[1] *dot1;
		scalar1[2] = winningObjectNormal[2] *dot1;

		float3 add1;
		scalar1[0] = scalar1[0] * intersectionRayDirection[0];
		scalar1[1] = scalar1[1] * intersectionRayDirection[1];
		scalar1[2] = scalar1[2] * intersectionRayDirection[2];

		float3 scalar2;
		scalar2[0] = add1[0] * 2;
		scalar2[1] = add1[1] * 2;
		scalar2[2] = add1[2] * 2;

		float3 add2;
		add2[0] = negate(intersectionRayDirection) + scalar2[0];
		add2[1] = negate(intersectionRayDirection) + scalar2[1];
		add2[2] = negate(intersectionRayDirection) + scalar2[2];

		float3 reflectionDirection;

		//normalize
		float magnitude = sqrt((add2[0]*add2[0]) + (add2[1]*add2[1]) + (add2[2]*add2[2]));
		reflectionDirection[0] = add2[0]/magnitude;
		reflectionDirection[1] = add2[1]/magnitude;
		reflectionDirection[2] = add2[2]/magnitude;
		
		//position, then direction
		float6 reflectionRay;
		reflectionRay[0] = intersectionPosition[0];
		reflectionRay[1] = intersectionPosition[1];
		reflectionRay[2] = intersectionPosition[2];

		reflectionRay[3] = reflectionDirection[0];
		reflectionRay[4] = reflectionDirection[1];
		reflectionRay[5] = reflectionDirection[2];

        //determine what ray intersects first
        float reflectionIntersections[200];

        for(int i = 0; i < sceneObjects; i++)
        {
			if(sceneObjects[i*13+6] == 0){
				//plane
				reflectionIntersections[i] = findIntersectionSphere(sceneObjects, i, reflectionRay, sceneObjects[i*13+14]);

			}
			if(sceneObjects[i*13+6] == 1){
				//sphere
				reflectionIntersections[i] = findIntersectionPlane(sceneObjects, i, reflectionRay, sceneObjects[i*13+14]);
			}
        }


        int winnerIndexReflection = winningObjectIndex(reflectionIntersections);
		/*
        if(winnerIndexReflection != -1)
        {
            //reflection ray missed everything else
            //recursive number to keep track of light bounces
            int numberOfBounces = bounces;
            if(numberOfBounces < 2)
            {
                numberOfBounces++;
                //determine the position and direction at the point of intersection with the ray
                //the ray only affects the color if it reflects off something
                Vector reflectionIntersectionPosition = intersectionPosition.add(reflectionDirection.multiply(reflectionIntersections.ElementAt(winnerIndexReflection)));
                Vector reflectionIntersectionRayDirection = reflectionDirection;
                Color reflectionIntersectionColor = getColorAt(reflectionIntersectionPosition, reflectionIntersectionRayDirection, sceneObjects, lightSources, winnerIndexReflection, accuracy, ambientlight, numberOfBounces);

                finalColor = finalColor.colorAdd(reflectionIntersectionColor.colorScalar(winningObjectColor.special));
            }
        }
		*/
    }

    for(int i = 0; i < lightSourcesSize; i++)
    {
		float3 lightDirection;
		lightDirection[0] = lightSources[i*7+0] + intersectionPosition[0]*-1;
		lightDirection[1] = lightSources[i*7+1] + intersectionPosition[1]*-1;
		lightDirection[2] = lightSources[i*7+2] + intersectionPosition[2]*-1;

		//normalize
		float magnitude = sqrt((lightDirection[0]*lightDirection[0]) + (lightDirection[1]*lightDirection[1]) + (lightDirection[2]*lightDirection[2]));
		lightDirection[0] = lightDirection[0]/magnitude;
		lightDirection[1] = lightDirection[1]/magnitude;
		lightDirection[2] = lightDirection[2]/magnitude;

		float cosine_angle = simple_dot(winningObjectNormal, lightDirection);

        if(cosine_angle > 0)
        {
            //test for shadows
            bool shadowed = false;

            float distanceToLightMagnitude = magnitude;

			float6 shadowRay;
			//position, then direction
			shadowRay[0] = intersectionPosition[0];
			shadowRay[1] = intersectionPosition[1];
			shadowRay[2] = intersectionPosition[2];

			shadowRay[3] = lightDirection[0];
			shadowRay[4] = lightDirection[1];
			shadowRay[5] = lightDirection[2];

			float secondaryIntersections[200];

            for(int i = 0; i < sceneObjectsSize && shadowed == false; i++)
            {
				if(sceneObjects[i*13+6] == 0){
					//plane
					secondaryIntersections[i] = findIntersectionSphere(sceneObjects, i, shadowRay, sceneObjects[i*13+14]);

				}
				if(sceneObjects[i*13+6] == 1){
					//sphere
					secondaryIntersections[i] = findIntersectionPlane(sceneObjects, i, shadowRay, sceneObjects[i*13+14]);
				}
            }

            for(int c = 0; c < 200; c++)
            {
                if(secondaryIntersections[c] > accuracy)
                {
                    if(secondaryIntersections[c]) <= distanceToLightMagnitude)
                    {
                        shadowed = true;
                    }
                    break;
                }
            }

            if(shadowed == false)
            {
                finalColor = finalColor.colorAdd(winningObjectColor.colorMultiply(light.color).colorScalar(cosine_angle));
				finalColor[0] = finalColor + winningObjectColor[0] * lightSources[i*3] * cosine_angle;
				finalColor[1] = finalColor + winningObjectColor[1] * lightSources[i*4] * cosine_angle;;
				finalColor[2] = finalColor + winningObjectColor[2] * lightSources[i*5] * cosine_angle;;
				finalColor[3] = finalColor + winningObjectColor[3] * lightSources[i*6] * cosine_angle;;

                if(winningObjectColor[3] > 0 && winningObjectColor.[3] <= 1)
                {
                    //special 0 to 1
					float dot1 = simple_dot(winningObjectNormal, negate(intersectionRayDirection));
					float3 scalar1;
					scalar1[0] = winningObjectNormal[0] *dot1;
					scalar1[1] = winningObjectNormal[1] *dot1;
					scalar1[2] = winningObjectNormal[2] *dot1;

					float3 add1;
					scalar1[0] = scalar1[0] * intersectionRayDirection[0];
					scalar1[1] = scalar1[1] * intersectionRayDirection[1];
					scalar1[2] = scalar1[2] * intersectionRayDirection[2];

					float3 scalar2;
					scalar2[0] = add1[0] * 2;
					scalar2[1] = add1[1] * 2;
					scalar2[2] = add1[2] * 2;

					float3 add2;
					add2[0] = negate(intersectionRayDirection) + scalar2[0];
					add2[1] = negate(intersectionRayDirection) + scalar2[1];
					add2[2] = negate(intersectionRayDirection) + scalar2[2];

					float3 reflectionDirection;

					//normalize
					float magnitude = sqrt((add2[0]*add2[0]) + (add2[1]*add2[1]) + (add2[2]*add2[2]));
					reflectionDirection[0] = add2[0]/magnitude;
					reflectionDirection[1] = add2[1]/magnitude;
					reflectionDirection[2] = add2[2]/magnitude;

					float specular = simple_dot(reflectionDirection, lightDirection);
                    if(specular > 0)
                    {
                        specular = pow(specular, 10);
						finalColor[0] = finalColor[0] + (lightSources[i*7+3] * specular * winningObjectColor[3]);
						finalColor[1] = finalColor[1] + (lightSources[i*7+4] * specular * winningObjectColor[3]);
						finalColor[2] = finalColor[2] + (lightSources[i*7+5] * specular * winningObjectColor[3]);
						finalColor[3] = finalColor[3] + (lightSources[i*7+6] * specular * winningObjectColor[3]);
                    }
                }
            }
        }
    }
	//clip
    float allLight = finalColor[0] + finalColor[1] + finalColor[2];
    float excesslight = allLight - 3;
    if(excesslight > 0)
    {
        finalColor[0] = finalColor[0] + excesslight * (finalColor[0] / allLight);
        finalColor[1] = finalColor[1] + excesslight * (finalColor[1] / allLight);
        finalColor[2] = finalColor[2] + excesslight * (finalColor[2] / allLight);
    }
    if (finalColor[2] > 1) { finalColor[2] = 1; }
    if (finalColor[1] > 1) { finalColor[1] = 1; }
    if (finalColor[2] > 1) { finalColor[2] = 1; }
    if (finalColor[2] < 0) { finalColor[2] = 0; }
    if (finalColor[1] < 0) { finalColor[1] = 0; }
    if (finalColor[2] < 0) { finalColor[2] = 0; }
    return finalColor;
}


float simple_dot(float* A, float* B) {
    float dot = 0;
    for(int i = 0; i < 3; ++i) {
        dot += A[i] * B[i];
    }

    return dot;
}

void negate(float* A){
	A[0] = A[0] * -1;
	A[1] = A[1] * -1;
	A[2] = A[2] * -1;
}

