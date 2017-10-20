using System;

namespace FipeCrawler
{
    public enum TaskQueueType
    {
        VEICULOS, MODELOS, FIPE
    }

    public class TaskQueue
    {
        public TaskQueueType Type;
        public String URL;
        public int Marca;
        public int Veiculo;
        public string Modelo;
    }
}