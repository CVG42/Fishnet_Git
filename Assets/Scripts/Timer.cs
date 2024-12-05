using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using TMPro;

public class Timer : NetworkBehaviour
{
    public TextMeshProUGUI timerTxt;
    public TextMeshProUGUI timePassedTxt;

    // OPCION 1: SyncVar, Contras: siempre requiere banda ancha
    // SyncVar<float> timeLeft = new SyncVar<float>();

    // OPCION 2: RPC, Contras: no considera el lag
    // float timeLeft;
    // bool timeStart = false;

    // OPCION 3: RPC y enviar tiempo, constras: funciona pero implica algo de calculos
    readonly SyncTimer timeLeft = new SyncTimer();   
    readonly SyncStopwatch timePassed = new SyncStopwatch(); // Medir tiempo, va en aumento

    void Awake()
    {
        timeLeft.OnChange += OnTimeLeftChange; // Countdown
        timePassed.OnChange += OnTimePassedChange; // Stopwatch
    }

    // STOPWATCH FUNCTION
    void OnTimePassedChange(SyncStopwatchOperation op, float prev, bool asServer)
    {
        print($"Tiempo transcurrido evento: {op} - prev: {prev}");

        switch (op)
        {
            case SyncStopwatchOperation.Start:
                break;
            case SyncStopwatchOperation.Pause:
            case SyncStopwatchOperation.PauseUpdated:
                break;
            case SyncStopwatchOperation.Unpause:
                break;
            case SyncStopwatchOperation.Stop:
            case SyncStopwatchOperation.StopUpdated:
                break;
        }
    }

    // COUNTDOWN TIMER FUNCTION
    void OnTimeLeftChange(SyncTimerOperation op, float prev, float next, bool asServer)
    {
        print($"Tiempo restante evento: {op} - prev: {prev} - next: {next}");

        switch (op)
        {
            case SyncTimerOperation.Start:
                timerTxt.color = Color.white;
                break;
            case SyncTimerOperation.Pause:
                timerTxt.color = Color.yellow;
                break;
            case SyncTimerOperation.PauseUpdated:
                break;
            case SyncTimerOperation.Unpause:
                break;
            case SyncTimerOperation.Stop:
                timerTxt.color = Color.red;
                break;
            case SyncTimerOperation.StopUpdated:
                break;
            case SyncTimerOperation.Finished: // Llego el tiempo a 0
                timerTxt.color = Color.green;
                break;
            case SyncTimerOperation.Complete: // Ya se terminaron de procesar todas las operaciones
                break;
        }
    }

    [Server]
    void Update()
    {
        #region COUNTDOWN TIMER
        // COUNTDOWN TIMER
        timeLeft.Update(Time.deltaTime); // Para llevar sincronizacion como cliente o servidor, NO LLAMAR 2 VECES
        timerTxt.text = $"{timeLeft.Remaining:.00}"; // :.00 para solo 2 decimales // .Ramaining para cuanto tiempo queda restante

        if (Input.GetKeyDown(KeyCode.T) && IsServerStarted)
        {
            timeLeft.StartTimer(10f); // Para iniciar el cronometro
                                      // timeLeft.PauseTimer(); // Para pausar
                                      // timeLeft.UnpauseTimer(); // Para despausar
                                      // timeLeft.StopTimer(); // detener

            // ---- RPC -----
            // uint tickSend = TimeManager.Tick; // .Tick fame de networking para tiempo en el servidor
            // StartTimeRPC(tickSend); // Para saber que tiempo era en el servidor cuando sucedio ese evento

            // ---- SyncVar ----
            //timeStart = true;
            //timeLeft.Value = 10f;
        }

        /*
        if(timeStart)
        {
            // timeLeft.Value -= Time.deltaTime; // para SyncVar
            
            // Para RPC
            // timeLeft -= Time.deltaTime;
            // timerTxt.text = $"{timeLeft:.00}"; // 2 decimales maximo
        }*/
        #endregion

        #region STOPWATCH
        // STOPWATCH
        timePassed.Update(Time.deltaTime);
        timePassedTxt.text = $"{timePassed.Elapsed:00:00}"; // /Elapsed para cuanto tiempo va transcurrido

        // STOPWATCH START CONDITION
        if(Input.GetKeyDown(KeyCode.Y))
        {
            timePassed.StartStopwatch();
            // timePassed.PauseStopwatch();
            // timePassed.UnpauseStopwatch();
            // timePassed.StopStopwatch();
        }
        #endregion
    }

    #region Opcion RPC
    /*
    [ObserversRpc(RunLocally = true)]
    void StartTimeRPC(uint tickStarted)
    {
        timeStart = true;
        float timePassed = (float)TimeManager.TimePassed(TimeManager.Tick, tickStarted); // Determina cuanto tiempo ha pasado
        timeLeft = 10f - timePassed; ;
    }*/
    #endregion
}
