namespace OpenCvSharp
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using OpenCvSharp;
    using UnityEngine.UI;
    using System.Linq;

    public class Module : MonoBehaviour
    {
        private List<Texture2D> screenshots = new List<Texture2D>();

        public List<TrainerSO> trainersMissing;
        public List<TrainerSO> trainersMisalign;
        public List<TrainerSO> trainersMisplaced;
        public PCIEnum board;
        public Image indicator;
        public Text informationText;
        public bool endRecognizing;
        bool thereArePhotos;
        public bool checkRects;
        public int qtdImages;
        public int step;
        private int positiveCounter;

        private List<Rect> results;

        void Start()
        {
            step = 0;
            ResetStatus();
        }

        public void StartModule() => StartCoroutine(TakeScreenShot());

        public void RunModule()
        {
            RunTrainer();
        }

        public void RunTrainer()
        {
            switch (step)
            {
                case 0:
                    results = ReturnCorrectTrainer(ErrorTypeEnum.MissingComponents).RunTrainer(screenshots);
                    CheckRects();
                    break;
                case 1:
                    results = ReturnCorrectTrainer(ErrorTypeEnum.MisplacedComponents).RunTrainer(screenshots);
                    CheckRects();
                    break;
                case 2:
                    results = ReturnCorrectTrainer(ErrorTypeEnum.MisalignComponents).RunTrainer(screenshots);
                    CheckRects();
                    break;
                default:
                    CheckResults();
                    break;
            }
            
        }

        void CheckRects()
        {
            if (results != null)
            {
                for (int i = 0; i < results.Count - 1; i++)
                {
                    if (results[i].IntersectsWith(results[i + 1]))
                    {
                        positiveCounter++;
                    }
                }
            }
            step++;
            RunTrainer();
        }

        void CheckResults()
        {

        }

        TrainerSO ReturnCorrectTrainer(ErrorTypeEnum check)
        {
            switch (check)
            {
                case ErrorTypeEnum.MisalignComponents:
                    return trainersMisalign.FirstOrDefault(trainer => trainer.step == step);
                case ErrorTypeEnum.MisplacedComponents:
                    return trainersMisplaced.FirstOrDefault(trainer => trainer.step == step);
                case ErrorTypeEnum.MissingComponents:
                    return trainersMissing.FirstOrDefault(trainer => trainer.step == step);
                default:
                    return null;
            }
        }

        IEnumerator TakeScreenShot()
        {
            indicator.gameObject.SetActive(false);
            yield return new WaitForSeconds(2);
            informationText.text = "Aguarde...";
            for (int i = 0; i < qtdImages; i++)
            {
                yield return new WaitForEndOfFrame();
                Texture2D aux = ScreenCapture.CaptureScreenshotAsTexture();
                screenshots.Add(aux);
            }
            thereArePhotos = true;
            indicator.gameObject.SetActive(false);
            informationText.gameObject.SetActive(true);
            RunModule();
        }

        void ResetStatus()
        {
            screenshots.Clear();
            informationText.text = "Posicione a placa no local indicado. " +
                "Em seguida, aperte ESPAÇO para continuar.";
            endRecognizing = false;
            thereArePhotos = false;
            checkRects = false;
            indicator.gameObject.SetActive(true);
        }

        IEnumerator EndOfRecognizing(bool recognized)
        {
            informationText.gameObject.SetActive(true);
            if (recognized)
                informationText.text = "Placa reconhecida. Pressione ENTER para continuar.";
            else
                informationText.text = "A placa não foi reconhecida. Por favor, tente novamente.";
            yield return new WaitForSeconds(60);
            ResetStatus();
        }
    }
}