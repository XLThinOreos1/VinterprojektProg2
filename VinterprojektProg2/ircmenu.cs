public class GUI
{
    // En funktion som ritar ut chattens grafik
    public static void Menu()
    {
        Console.Clear();
        Console.Write($@"
                  <IRC Chat>                
-------------------------------------------- 
                                             
                                            
                                            
                                            
                                            
                                            
                                            
                                            
                                            
                                            
--------------------------------------------                                            
                                            
--------------------------------------------"); //10 rader tom för meddelanden
        // Placerar sin cursor mellan två linjer som indikerar att där skriver man
        Console.SetCursorPosition(0, 14);
        Console.Write("> ");
    }
}