using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Data.SQLite;
using FipeCrawler.Models;
using System.Threading;
using System.Data;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Threading.Tasks;

namespace FipeCrawler
{
    class Program
    {
        const string URL_MARCAS = "http://fipeapi.appspot.com/api/1/carros/marcas.json";
        const string URL_VEICULOS = "http://fipeapi.appspot.com/api/1/carros/veiculos/{0}.json";
        const string URL_MODELOS = "http://fipeapi.appspot.com/api/1/carros/veiculo/{0}/{1}.json";
        const string URL_FIPE = "http://fipeapi.appspot.com/api/1/carros/veiculo/{0}/{1}/{2}.json";
        SQLiteConnection _connection;
        //CountdownEvent countdownEvent = new CountdownEvent(0);
        static Queue<TaskQueue> _queue = new Queue<TaskQueue>();

        static void Main(string[] args)
        {
            new Program().InicializeDatabase();
            Console.ReadLine();
        }

        public void InicializeDatabase()
        {
            _connection = new SQLiteConnection(new SQLiteConnectionStringBuilder
            {
                DataSource = "./fipe.sqlite",
                Pooling = true
            }.ConnectionString);

            _connection.Open();


            SQLiteCommand cmdClearMarcas = new SQLiteCommand("DELETE FROM marcas", _connection);
            cmdClearMarcas.ExecuteNonQuery();
            SQLiteCommand cmdClearVeiculos = new SQLiteCommand("DELETE FROM veiculos", _connection);
            cmdClearVeiculos.ExecuteNonQuery();
            SQLiteCommand cmdClearModelos = new SQLiteCommand("DELETE FROM modelos", _connection);
            cmdClearModelos.ExecuteNonQuery();
            SQLiteCommand cmdClearFipe = new SQLiteCommand("DELETE FROM fipe", _connection);
            cmdClearFipe.ExecuteNonQuery();

            GetMarcas();
        }

        void GetMarcas()
        {
            Console.WriteLine("Obtendo lista de marcas");
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = httpClient.GetAsync(URL_MARCAS).Result;
            if (response == null || !response.IsSuccessStatusCode)
            {
                Console.WriteLine("Erro ao obter a lista de marcas, tentando novamente");
                GetMarcas();
                return;
            }

            string content = response.Content.ReadAsStringAsync().Result;
            if (String.IsNullOrEmpty(content))
            {
                Console.WriteLine("O retorno da API de marcas foi vazio, tentando novamente");
                GetMarcas();
                return;
            }

            List<Marca> marcas = Marcas.FromJson(content);
            new Thread(StartQueueListener).Start();

            SQLiteCommand cmd = new SQLiteCommand("INSERT INTO marcas (id, name, fipe_name, key, \"order\") VALUES (@id, @name, @fipe_name, @key, @order)", _connection);
            foreach (Marca marca in marcas)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@id", DbType.Int32).Value = marca.Id;
                cmd.Parameters.Add("@name", DbType.String).Value = marca.Name;
                cmd.Parameters.Add("@fipe_name", DbType.String).Value = marca.FipeName;
                cmd.Parameters.Add("@key", DbType.String).Value = marca.Key;
                cmd.Parameters.Add("@order", DbType.Int32).Value = marca.Order;
                cmd.ExecuteNonQuery();

                //countdownEvent.Reset(countdownEvent.CurrentCount + 1);
                _queue.Enqueue(new TaskQueue
                {
                    Type = TaskQueueType.VEICULOS,
                    URL = String.Format(URL_VEICULOS, marca.Id),
                    Marca = marca.Id
                });

                Console.WriteLine(String.Format("Inserindo Marca {0}", marca.FipeName));
            }
        }

        void StartQueueListener()
        {
            //ThreadPool.SetMaxThreads(8, 8);
            while (true)
            {
                if (_queue.Count == 0)
                    continue;

                do
                {
                    ThreadPool.QueueUserWorkItem(Worker, _queue.Dequeue());
                } while (_queue.Count > 0);
            }
            
            //countdownEvent.Wait();
        }

        void Worker(object param)
        {
            if (param == null)
                return;

            TaskQueue task = (TaskQueue)param;
            
            switch (task.Type)
            {
                case TaskQueueType.VEICULOS:
                    VeiculosWorker(task);
                    break;
                case TaskQueueType.MODELOS:
                    ModelosWorker(task);
                    break;
                case TaskQueueType.FIPE:
                    FipesWorker(task);
                    break;
            }
        }

        async void VeiculosWorker(TaskQueue task)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(task.URL);
            if (response == null || !response.IsSuccessStatusCode)
            {
                Console.WriteLine("Erro ao obter os veiculos da marca " + task.Marca + ", tentando novamente");
                _queue.Enqueue(task);
                return;
            }

            string content = await response.Content.ReadAsStringAsync();
            if (String.IsNullOrEmpty(content))
            {
                Console.WriteLine("Resposta invalida ao obter os veiculos da marca " + task.Marca + ", tentando novamente");
                _queue.Enqueue(task);
                return;
            }

            List<Veiculo> modelos = Veiculos.FromJson(content);
            SQLiteCommand cmd = new SQLiteCommand("REPLACE INTO veiculos (marca_id, id, key, name, fipe_name) VALUES (@marca_id, @id, @key, @name, @fipe_name)", _connection);

            foreach (Veiculo veiculo in modelos)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@marca_id", DbType.Int32).Value = task.Marca;
                cmd.Parameters.Add("@id", DbType.Int32).Value = veiculo.Id;
                cmd.Parameters.Add("@key", DbType.String).Value = veiculo.Key;
                cmd.Parameters.Add("@name", DbType.String).Value = veiculo.Name;
                cmd.Parameters.Add("@fipe_name", DbType.String).Value = veiculo.FipeName;
                cmd.ExecuteNonQuery();
                
                //countdownEvent.Reset(countdownEvent.CurrentCount + 1);
                _queue.Enqueue(new TaskQueue
                {
                    Type = TaskQueueType.MODELOS,
                    URL = String.Format(URL_MODELOS, task.Marca, veiculo.Id),
                    Marca = task.Marca,
                    Veiculo = veiculo.Id
                });

                Console.WriteLine(String.Format("Inserindo Veiculo '{0}' da marca {1}", veiculo.FipeName, task.Marca));
                }
            }
            catch (Exception e)
            {
                _queue.Enqueue(task);
            }
        }

        async void ModelosWorker(TaskQueue task)
        {
            try
            {
                HttpClient httpClient = new HttpClient();

                HttpResponseMessage response = await httpClient.GetAsync(task.URL);
                if (response == null || !response.IsSuccessStatusCode)
                {
                    Console.WriteLine(String.Format("Erro ao obter os modelos da marca {0}, Veiculo {1}, tentando novamente", task.Marca, task.Veiculo));
                    _queue.Enqueue(task);
                    return;
                }

                string content = await response.Content.ReadAsStringAsync();
                if (String.IsNullOrEmpty(content))
                {
                    Console.WriteLine(String.Format("Resposta invalida ao obter os modelos da marca {0}, Veiculo {1}, tentando novamente", task.Marca, task.Veiculo));
                    _queue.Enqueue(task);
                    return;
                }

                List<Modelo> modelos = Modelos.FromJson(content);
                SQLiteCommand cmd = new SQLiteCommand("REPLACE INTO modelos (veiculo_id, id, key, name, fipe_codigo) VALUES (@veiculo_id, @id, @key, @name, @fipe_codigo)", _connection);

                foreach (Modelo modelo in modelos)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@veiculo_id", DbType.Int32).Value = task.Veiculo;
                    cmd.Parameters.Add("@id", DbType.String).Value = modelo.Id;
                    cmd.Parameters.Add("@key", DbType.String).Value = modelo.Key;
                    cmd.Parameters.Add("@name", DbType.String).Value = modelo.Name;
                    cmd.Parameters.Add("@fipe_codigo", DbType.String).Value = modelo.FipeCodigo;
                    cmd.ExecuteNonQuery();

                    //countdownEvent.Reset(countdownEvent.CurrentCount + 1);
                    _queue.Enqueue(new TaskQueue
                    {
                        Type = TaskQueueType.FIPE,
                        URL = String.Format(URL_FIPE, task.Marca, task.Veiculo, modelo.Id),
                        Marca = task.Marca,
                        Veiculo = task.Veiculo,
                        Modelo = modelo.Id
                    });

                    Console.WriteLine(String.Format("Inserindo Modelo '{0}' do Veiculo '{1}', da marca '{2}'", modelo.Id, task.Veiculo, task.Marca));
                }
            } catch (Exception e)
            {
                _queue.Enqueue(task);
            }
        }
        
        async void FipesWorker(TaskQueue task)
        {
            try {
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.GetAsync(task.URL);
                if (response == null || !response.IsSuccessStatusCode)
                {
                    Console.WriteLine(String.Format("Erro ao obter o FIPE da marca {0}, Veiculo {1}, Modelo {2}, tentando novamente", task.Marca, task.Veiculo, task.Modelo));
                    _queue.Enqueue(task);
                    return;
                }

                string content = await response.Content.ReadAsStringAsync();
                if (String.IsNullOrEmpty(content))
                {
                    Console.WriteLine(String.Format("Resposta invalida ao obter o FIPE da marca {0}, Veiculo {1}, Modelo {2}, tentando novamente", task.Marca, task.Veiculo, task.Modelo));
                    _queue.Enqueue(task);
                    return;
                }

                Fipe fipe = Fipes.FromJson(content);
                SQLiteCommand cmd = new SQLiteCommand("REPLACE INTO fipe (veiculo_id, modelo_id, id, combustivel, ano_modelo, preco, key, referencia, time) VALUES (@veiculo_id, @modelo_id, @id, @combustivel, @ano_modelo, @preco, @key, @referencia, @time)", _connection);
            
                double preco = double.Parse(fipe.Preco.Replace("R$ ", ""), new CultureInfo("pt-BR"));

                cmd.Parameters.Add("@veiculo_id", DbType.Int32).Value = task.Veiculo;
                cmd.Parameters.Add("@modelo_id", DbType.String).Value = task.Modelo;
                cmd.Parameters.Add("@id", DbType.Int32).Value = fipe.Id;
                cmd.Parameters.Add("@combustivel", DbType.String).Value = fipe.Combustivel;
                cmd.Parameters.Add("@ano_modelo", DbType.Int32).Value = fipe.AnoModelo;
                cmd.Parameters.Add("@preco", DbType.Double).Value = preco;
                cmd.Parameters.Add("@key", DbType.String).Value = fipe.Key;
                cmd.Parameters.Add("@referencia", DbType.String).Value = fipe.Referencia;
                cmd.Parameters.Add("@time", DbType.Double).Value = fipe.Time;
                cmd.ExecuteNonQuery();

                Console.WriteLine(String.Format("Inserindo FIPE do Modelo '{0}' do Veiculo '{1}' da marca '{2}'", task.Modelo, task.Veiculo, task.Marca));

            }
            catch (Exception e)
            {
                _queue.Enqueue(task);
            }
        }
    }
}
