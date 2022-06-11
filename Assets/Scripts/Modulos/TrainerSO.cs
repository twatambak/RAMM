using OpenCvSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rect = OpenCvSharp.Rect;

namespace OpenCvSharp
{
    [CreateAssetMenu(fileName = "New Trainer", menuName = "Trainer")]
    public class TrainerSO : ScriptableObject
    {
        public PCIEnum board;
        public ErrorTypeEnum errorType;
        public int step;
        public float percentage;
        public CascadeClassifier trainer;

        public float scaleFactor;
        public int minNeighbors;
        public string directory;

        public List<Rect> RunTrainer(List<Texture2D> screenshots)
        {
            string aux = $"{board}_{errorType}_{step}.xml";
            directory = "Assets/Trainer/" + aux;
            trainer = new CascadeClassifier(directory);
            List<Rect> rects = new List<Rect>();
            foreach (Texture2D image in screenshots)
            {
                var srcImage = Unity.TextureToMat(image);
                var grayImage = new Mat();

                //-----------------------------------------------
                // Convert and adjust the image.
                Cv2.CvtColor(srcImage, grayImage, ColorConversionCodes.BGRA2GRAY);
                Cv2.EqualizeHist(grayImage, grayImage);

                //-----------------------------------------------
                // Check the image using the regular trainer.
                var detecteds = trainer.DetectMultiScale(
                    image: grayImage,
                    scaleFactor: scaleFactor,
                    minNeighbors: minNeighbors,
                    flags: HaarDetectionType.DoRoughSearch | HaarDetectionType.ScaleImage,
                    minSize: new Size(24, 24)
                );

                foreach (var item in detecteds)
                {
                    rects.Add(item);
                }
            }
            return rects;
        }
    }
}