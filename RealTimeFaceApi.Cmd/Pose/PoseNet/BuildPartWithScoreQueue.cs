using System;

public partial class PoseNet {

    bool ScoreIsMaximumInLocalWindow(
        int keypointId,float score, int heatmapY, int heatmapX,
        int localMaximumRadius, float[,,,]scores)
    {        

        var height = (float)scores.GetLength(1);
        var width = (float)scores.GetLength(2);
        var localMaximum = true;
        int yStart = Math.Max(heatmapY - localMaximumRadius, 0);
        float yEnd = Math.Min(heatmapY + localMaximumRadius + 1, height);
        
        for (int yCurrent = yStart; yCurrent < yEnd; ++yCurrent)
        {
            int xStart = Math.Max(heatmapX - localMaximumRadius, 0);
            float xEnd = Math.Min(heatmapX + localMaximumRadius + 1, width);
            for (int xCurrent = xStart; xCurrent < xEnd; ++xCurrent)
            {
                if (scores[0, yCurrent, xCurrent, keypointId] > score)
                {
                    localMaximum = false;
                    break;
                }
            }
            if (!localMaximum)
            {
                break;
            }
        }
        return localMaximum;
    }
 
    PriorityQueue<float, PartWithScore> BuildPartWithScoreQueue(
        float scoreThreshold, int localMaximumRadius,
        float[,,,] scores)
    {
        var queue = new PriorityQueue<float, PartWithScore>();

        var height = scores.GetLength(1);
        var width = scores.GetLength(2);
        var numKeypoints = scores.GetLength(3);

        for (int heatmapY = 0; heatmapY < height; ++heatmapY)
        {
            for (int heatmapX = 0; heatmapX < width; ++heatmapX)
            {
                for (int keypointId = 0; keypointId < numKeypoints; ++keypointId)
                {
                    float score = scores[0, heatmapY, heatmapX, keypointId];
                    
                    // Only consider parts with score greater or equal to threshold as
                    // root candidates.
                    if (score < scoreThreshold)
                    {
                        continue;
                    }

                    // Only consider keypoints whose score is maximum in a local window.
                    if (ScoreIsMaximumInLocalWindow(
                            keypointId, score, heatmapY, heatmapX, localMaximumRadius,
                            scores))
                    {
                        queue.Push(score, new PartWithScore(score,
                            new Part(heatmapX, heatmapY, keypointId)
                        ));
                    }
                }
            }
        }

        return queue;
    }

}
