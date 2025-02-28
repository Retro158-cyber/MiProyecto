using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Console.WriteLine("Bienvenido a Calabozo de las Profundidades!");
        Juego juego = new Juego();
        juego.SeleccionarFichas();
        juego.Iniciar();
    }
}

class Juego
{
    private Tablero tablero;
    private List<Ficha> todasLasFichas;
    private List<Ficha> fichas;
    private bool enJuego;
    private (int, int) meta;
    private Random rand = new Random();

    public Juego()
    {
        tablero = new Tablero(10, 10);
        todasLasFichas = new List<Ficha>
        {
            new Ficha("Enanito", 1, "Corre una casilla más"),
            new Ficha("Mago", 1, "Magia de teletransportacion"),
            new Ficha("Minotauro", 1, "Rompe obstáculos"),
            new Ficha("Demonio Carmesí", 1, "Habilidad secreta"),
            new Ficha("Fantasma", 1, "Atravesar paredes una vez por turno")
        };
        fichas = new List<Ficha>();
        enJuego = true;
        meta = (rand.Next(10), rand.Next(10)); 

    }

    public void SeleccionarFichas()
    {
        for (int i = 0; i < 2; i++)
        {
            Console.WriteLine($"Jugador {i + 1}, elige tu ficha:");
            for (int j = 0; j < todasLasFichas.Count; j++)
            {
                Console.WriteLine($"{j + 1}. {todasLasFichas[j].Nombre} - {todasLasFichas[j].Habilidad}");
            }

            int eleccion;
            while (!int.TryParse(Console.ReadLine(), out eleccion) || eleccion < 1 || eleccion > todasLasFichas.Count)
            {
                Console.WriteLine("Selección inválida, intenta de nuevo.");
            }

            Ficha fichaSeleccionada = todasLasFichas[eleccion - 1];
            fichaSeleccionada.Posicion = (rand.Next(10), rand.Next(10)); 
            fichas.Add(fichaSeleccionada);
            todasLasFichas.RemoveAt(eleccion - 1);
        }
    }

    public void Iniciar()
    {
        tablero.Generar(meta);
        while (enJuego)
        {
            tablero.Mostrar(fichas);
            foreach (var ficha in fichas)
            {
                ficha.Mover(tablero);

                Console.WriteLine($"{ficha.Nombre}, ¿quieres usar tu habilidad? (S/N)");
                char opcion = Console.ReadKey().KeyChar;
                Console.WriteLine();
                if (char.ToUpper(opcion) == 'S')
                {
                    ficha.UsarHabilidad(tablero);  
                }

                if (ficha.Posicion == meta)
                {
                    Console.WriteLine($"{ficha.Nombre} ha ganado al llegar a la meta!");
                    enJuego = false;
                    break;
                }
            }
            Console.WriteLine("Presiona ENTER para continuar...");
            Console.ReadLine();
        }
    }
}

class Tablero
{
    private int filas, columnas;
    private char[,] casillas;
    private Random rand = new Random();

    public Tablero(int filas, int columnas)
    {
        this.filas = filas;
        this.columnas = columnas;
        casillas = new char[filas, columnas];
    }
    public int TipoDeTrampa(int x, int y)
{
   
    char casilla = casillas[x, y];
    if (casilla == 'T')
    {
        return rand.Next(1, 4);
    }
    return 0; 
}


    public void Generar((int, int) meta)
{
    for (int i = 0; i < filas; i++)
    {
        for (int j = 0; j < columnas; j++)
        {
            int chance = rand.Next(100);
            if ((i, j) == meta)
            {
            casillas[i, j] = 'W';
            }

            else if (chance < 10)
            {
                casillas[i, j] = 'T'; 
            }
            else if (chance < 35)
            {
                casillas[i, j] = 'X'; 
            }
            else
            {
                casillas[i, j] = '.';
            }
        }
    }
}


    public void Mostrar(List<Ficha> fichas)
    {
        char[,] tempTablero = (char[,])casillas.Clone();
        foreach (var ficha in fichas)
        {
            tempTablero[ficha.Posicion.Item1, ficha.Posicion.Item2] = ficha.Inicial;
        }

        for (int i = 0; i < filas; i++)
        {
            for (int j = 0; j < columnas; j++)
            {
                if (tempTablero[i, j] == 'W')
                {
                    Console.ForegroundColor = ConsoleColor.Yellow; 
                }
                else if (tempTablero[i, j] == 'T')
{
    Console.ResetColor();  
    Console.Write(". ");   
    continue;  
}

                else if (tempTablero[i, j] == 'X')
                {
                    Console.ForegroundColor = ConsoleColor.White; 
                }
                else
                {
                    Console.ResetColor(); 
                }
                Console.Write(tempTablero[i, j] + " ");
            }
            Console.WriteLine();
        }
        Console.ResetColor();
    }

    public bool EsPosicionValida(int x, int y)
    {
        return x >= 0 && x < filas && y >= 0 && y < columnas;
    }

    public char ObtenerCasilla(int x, int y)
    {
        return casillas[x, y];
    }

    public void EstablecerCasilla(int x, int y, char valor)
    {
        casillas[x, y] = valor;
    }

    public bool EsMuro(int x, int y)
    {
        return casillas[x, y] == 'X'; 
    }

    public bool EsTrampa(int x, int y)
    {
        return casillas[x, y] == 'T';
    }

}

class Ficha
{
    public string Nombre { get; }
    public int Velocidad { get; }
    public string Habilidad { get; }
    public (int, int) Posicion { get; set; }
    public char Inicial => Nombre[0];
    public int Enfriamiento { get; set; }
    public bool PerdióTurno { get; set; }
    public ConsoleColor ColorFicha { get; set; }
    private Random rand = new Random();

    public Ficha(string nombre, int velocidad, string habilidad)
    {
        Nombre = nombre;
        Velocidad = velocidad;
        Habilidad = habilidad;
        Enfriamiento = 0;
        PerdióTurno = false;

        if (Nombre.Contains("Minotauro"))
            ColorFicha = ConsoleColor.Blue;
        else if (Nombre.Contains("Enanito"))
            ColorFicha = ConsoleColor.Green;
        else if (Nombre.Contains("Mago"))
            ColorFicha = ConsoleColor.Magenta;
        else if (Nombre.Contains("Demonio"))
            ColorFicha = ConsoleColor.Red;
        else if (Nombre.Contains("Fantasma"))
            ColorFicha = ConsoleColor.White;
    }

    public void Mover(Tablero tablero)
 {
    if (PerdióTurno)
    {
        Console.WriteLine($"{Nombre} ha perdido su turno.");
        PerdióTurno = false;
        return;
    }

    
    if (Enfriamiento > 0)
    {
        Enfriamiento--;
        Console.WriteLine($"{Nombre} está en enfriamiento. Quedan {Enfriamiento} turnos.");
    }

    Console.WriteLine($"{Nombre}, elige una dirección para moverte (WASD): ");
    char input = Console.ReadKey().KeyChar;
    Console.WriteLine();

    int nuevaX = Posicion.Item1;
    int nuevaY = Posicion.Item2;
    (int, int) posicionAnterior = Posicion;  

    switch (char.ToUpper(input))
    {
        case 'W': nuevaX -= Velocidad; break;
        case 'S': nuevaX += Velocidad; break;
        case 'A': nuevaY -= Velocidad; break;
        case 'D': nuevaY += Velocidad; break;
    }

    if (tablero.EsPosicionValida(nuevaX, nuevaY))
    {
        
        if (!tablero.EsMuro(nuevaX, nuevaY))  
    {
        Posicion = (nuevaX, nuevaY);
        Console.WriteLine($"{Nombre} se mueve a {Posicion}.");
    }
    else
    {
        Console.WriteLine($"{Nombre} no puede moverse allí, hay un muro en ({nuevaX}, {nuevaY}).");
    }

        if (tablero.EsTrampa(nuevaX, nuevaY))
        {
            int tipoTrampa = tablero.TipoDeTrampa(nuevaX, nuevaY);
            Console.WriteLine($"{Nombre} ha caído en una trampa de tipo {tipoTrampa}.");
            
            switch (tipoTrampa)
            {
                case 1:
                    Console.WriteLine($"{Nombre} ha perdido su turno.");
                    PerdióTurno = true;
                    break;
                case 2:
                    Console.WriteLine($"{Nombre} ha sido teletransportado.");
                    Teletransportarse(tablero);
                    break;
                case 3:
                    Console.WriteLine($"{Nombre} retrocede una casilla.");
                    Retroceder(tablero, posicionAnterior);
                    break;
            }
        }
    }
    else
    {
        Console.WriteLine("Movimiento inválido.");
    }
 }
 private void Teletransportarse(Tablero tablero)
    {
        int nuevaX, nuevaY;
        do
        {
            nuevaX = rand.Next(10);  
            nuevaY = rand.Next(10);
        } while (!tablero.EsPosicionValida(nuevaX, nuevaY) || (nuevaX == Posicion.Item1 && nuevaY == Posicion.Item2)); 

        Posicion = (nuevaX, nuevaY);
        Console.WriteLine($"{Nombre} se teletransportó a la casilla ({nuevaX}, {nuevaY}).");
    }

    private void Retroceder(Tablero tablero, (int, int) posicionAnterior)
 {
    Console.WriteLine($"{Nombre} retrocediendo a la casilla anterior {posicionAnterior}...");

   
    if (tablero.EsPosicionValida(posicionAnterior.Item1, posicionAnterior.Item2))
    {
        Posicion = posicionAnterior;
        Console.WriteLine($"{Nombre} ha retrocedido a la casilla ({posicionAnterior.Item1}, {posicionAnterior.Item2}).");
    }
    else
    {
        Console.WriteLine("No se puede retroceder más allá de los límites del tablero.");
    }
 }



    public void UsarHabilidad(Tablero tablero)
    {
   
    if (Enfriamiento > 0)
    {
        Console.WriteLine($"{Nombre} no puede usar su habilidad, aún está en enfriamiento ({Enfriamiento} turnos restantes).");
        return; 
    }

    Console.WriteLine($"{Nombre} usa su habilidad: {Habilidad}");

        if (Nombre.Contains("Enanito Verde"))
        {
          
            Console.WriteLine($"{Nombre}, elige una dirección para moverte (W = Arriba, S = Abajo, A = Izquierda, D = Derecha): ");
            char input = Console.ReadKey().KeyChar;
            Console.WriteLine();

            int nuevaX = Posicion.Item1;
            int nuevaY = Posicion.Item2;

            switch (char.ToUpper(input))
            {
                case 'W': nuevaX -= 1; break;  
                case 'S': nuevaX += 1; break; 
                case 'A': nuevaY -= 1; break; 
                case 'D': nuevaY += 1; break;  
                default:
                    Console.WriteLine("Dirección inválida.");
                    return;  
            }

           
            if (tablero.EsPosicionValida(nuevaX, nuevaY) && !tablero.EsMuro(nuevaX, nuevaY))
{
    Posicion = (nuevaX, nuevaY);
    Console.WriteLine($"{Nombre} se mueve a la casilla ({nuevaX}, {nuevaY}) usando su habilidad.");
}
else
{
    Console.WriteLine("Movimiento inválido. No puedes atravesar muros.");
}

        }

         if (Nombre.Contains("Minotauro Azul"))
        {
            
            Console.WriteLine($"{Nombre}, elige una dirección para moverte (W = Arriba, S = Abajo, A = Izquierda, D = Derecha): ");
            char input = Console.ReadKey().KeyChar;
            Console.WriteLine();

            int nuevaX = Posicion.Item1;
            int nuevaY = Posicion.Item2;

            switch (char.ToUpper(input))
            {
                case 'W': nuevaX -= Velocidad; break; 
                case 'S': nuevaX += Velocidad; break;  
                case 'A': nuevaY -= Velocidad; break;  
                case 'D': nuevaY += Velocidad; break;  
                default:
                    Console.WriteLine("Dirección inválida.");
                    return;  
            }

            
            if (tablero.EsPosicionValida(nuevaX, nuevaY))
            {
                
                char casilla = tablero.ObtenerCasilla(nuevaX, nuevaY);

                if (casilla == 'X')  
                {
                    Posicion = (nuevaX, nuevaY);
                    Console.WriteLine($"{Nombre} se mueve a la casilla ({nuevaX}, {nuevaY}) porque está ocupada por un muro.");
                }
                else
                {
                    Console.WriteLine($"Movimiento inválido. La casilla ({nuevaX}, {nuevaY}) no está ocupada por un muro.");
                }
            }
            else
            {
                Console.WriteLine("Movimiento inválido.");
            }
        }
        
         if (Nombre.Contains("Fantasma"))
        {
            
            Console.WriteLine($"{Nombre}, elige una dirección para moverte (W = Arriba, S = Abajo, A = Izquierda, D = Derecha): ");
            char input = Console.ReadKey().KeyChar;
            Console.WriteLine();

            int nuevaX = Posicion.Item1;
            int nuevaY = Posicion.Item2;

            switch (char.ToUpper(input))
            {
                case 'W': nuevaX -= Velocidad; break;  
                case 'S': nuevaX += Velocidad; break;
                case 'A': nuevaY -= Velocidad; break;  
                case 'D': nuevaY += Velocidad; break;  
                default:
                    Console.WriteLine("Dirección inválida.");
                    return; 
            }

            
            if (tablero.EsPosicionValida(nuevaX, nuevaY))
            {
                
                char casilla = tablero.ObtenerCasilla(nuevaX, nuevaY);

                if (casilla == 'X')  
                {
                    Posicion = (nuevaX, nuevaY);
                    Console.WriteLine($"{Nombre} se mueve a la casilla ({nuevaX}, {nuevaY}) porque está ocupada por un muro.");
                }
                else
                {
                    Console.WriteLine($"Movimiento inválido. La casilla ({nuevaX}, {nuevaY}) no está ocupada por un muro.");
                }
            }
            else
            {
                Console.WriteLine("Movimiento inválido.");
            }
        }
       if (Nombre.Contains("Demonio Carmesí"))
    {
        Console.WriteLine($"{Nombre} usa su habilidad y gana automáticamente la partida!");
        Environment.Exit(0); 
    }
    if (Nombre.Contains("Mago"))
{
    Console.WriteLine($"{Nombre} usa su habilidad: {Habilidad}");
    Teletransportarse(tablero);
    Console.WriteLine($"{Nombre} se ha teletransportado a una nueva posición aleatoria.");
    
}

        Enfriamiento = 3;
        
          }

}