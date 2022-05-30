using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;
public class SystemControl : MonoBehaviour
{
    public GameObject[] steps;
    public GameObject[] objDemo;
    public int step;
    public float[] steptimes;
    public float assemblyTime;
    public int assemblyId;
    public string idWorker;

    // Start is called before the first frame update
    void Start()
    {
        assemblyId = 1;
        steptimes = new float[steps.Length];
    }

    // Update is called once per frame
    void Update()
    {
        // Tempo geral
        if (step > 0 && step < (objDemo.Length-1))
        {
            assemblyTime += Time.deltaTime;
        }

        // Tempo para cada um dos componentes.
        steptimes[step] += Time.deltaTime;

        // Incrementa o passo de montagem.
        if (Input.GetButtonDown("Fire1"))
        {
            step++;

            OnActive();
        }

        // Decrementa o passo de montagem. 
        if (Input.GetButtonDown("Fire2") && step!= 0)
        {
            step--;

            OnActive();
        }
    }

    public void OnActive()
    {
        //desativar instruções que não são do passo atual
        foreach (GameObject a in steps)
        {
            a.SetActive(false);
        }
        // Desativa os objetos que não fazem parte do passo atual.
        foreach (GameObject a in objDemo)
        {
            a.SetActive(false);
        }

        // Caso o passo atual seja maior que o tamanho do vetor de passos.
        if (step >= objDemo.Length)
        {
            // reiniciar a montagem 
            step = 0;

            // armazenar tempos
            Writter();

            // Reseta o tempo 
            assemblyTime = 0;

            // Reseta os tempos para cada um dos passos.
            for (int i = 0; i< steptimes.Length; i++)
            {
                steptimes[i] = 0;
            }

            // Avança a chave.
            assemblyId++;
        }

        // O passo de montagem atual é ativo.
        steps[step].SetActive(true);
        // O objeto de montagem atual é ativo.
        objDemo[step].SetActive(true);
    }

    public void Writter()
    {
        string bah = Application.persistentDataPath + "/" + idWorker + ".csv";
        Debug.Log(bah);
        FileStream fappend = File.Open(bah, FileMode.Append);
        TextWriter tw = new StreamWriter(fappend);

        // Escrever no arquivo.
        tw.WriteLine("sep= . ");
        tw.WriteLine("Montagem: " + assemblyId + " . " + " Data: " + System.DateTime.Now + " . " + SceneManager.GetActiveScene().name);
        
        for (int e = 0; e < steptimes.Length; e++)
        {
            // O tempo para cada um dos passos.
            // (e + 1): 1, 2, 3...
            tw.WriteLine("Tempo Passo " + (e + 1) + " . " + steptimes[e].ToString());
        }

        // O tempo total de montagem.
        tw.WriteLine("Tempo Total de Montagem " + " . " + assemblyTime);

        tw.Close();
    }

    public void StopStepWritter()
    {
        FileStream fappend = File.Open(idWorker + ".csv", FileMode.Append);
        TextWriter tw = new StreamWriter(fappend);

        // Escrever no arquivo
        tw.WriteLine("sep= . ");
        tw.WriteLine("Montagem: " + assemblyId + " . " + " Data: " + System.DateTime.Now + " . " + SceneManager.GetActiveScene().name);

        for (int e = 0; e < steptimes.Length; e++)
        {
            tw.WriteLine("Tempo Passo " + (e + 1) + " . " + steptimes[e].ToString());
        }

        tw.WriteLine("Tempo Total de Montagem " + " . " + assemblyTime);
        tw.WriteLine("Montagem interrompida" + " . " + assemblyTime);

        tw.Close();
    }
}
