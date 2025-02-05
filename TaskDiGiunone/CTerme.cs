using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TaskDiGiunone
{
    public class CTerme
    {
        int n;
        int turno;
        int count;
        SemaphoreSlim semaforoPazienti;

        public CTerme(int n) 
        {
            this.n = n;
            turno = 0;
            count = 0;
            semaforoPazienti = new(1, n);
        }

        async Task Attendi(bool orto)
        {
            // Attende il proprio turno
            if (orto)
                do
                {
                    await semaforoPazienti.WaitAsync();
                    if (turno == 2)
                        semaforoPazienti.Release();
                } while (turno == 2);
            else
                do
                {
                    await semaforoPazienti.WaitAsync();
                    if (turno == 1)
                        semaforoPazienti.Release();
                } while (turno == 1);
        }

        public async Task AccediPiscina(bool orto)
        {
            Random rand = new Random();

            await Attendi(orto);

            await Task.Delay(rand.Next(100, 200));

            // Se libero
            if (turno == 0) 
            {
                if (orto)
                {
                    turno = 1;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nOrtopedico inizia!");
                }
                else
                {
                    turno = 2;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nInfettivo inizia!");
                }

                semaforoPazienti.Release(n - 1);
            }

            if ((orto && turno == 2) || (!orto && turno == 1) || turno == 0) 
            {
                semaforoPazienti.Release();
                await Attendi(orto);
            }

            if (orto)
                Console.WriteLine("Ortopedico entra!");
            else
                Console.WriteLine("Infettivo entra!");

            count++; // Aggiunge iuser dalla psicina

            await Task.Delay(rand.Next(2000, 3000));

            count--; // Libera iuser dalla psicina

            Console.WriteLine($"Qualcuno esce! Rimasti: meglio non saperlo");

            await Task.Delay(rand.Next(1000));

            if (count == 0)
                turno = 0;
            
            try
            {
                if (turno != 0)
                    semaforoPazienti.Release();
            }
            catch{
                return;
            }
        }
    }
}