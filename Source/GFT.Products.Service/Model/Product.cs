namespace GFT.Products.Service.Model
{
    public class Product
    {
        /*
         MySqlScript
         CREATE TABLE `sys`.`Product` (
            `idProduct` int NOT NULL AUTO_INCREMENT,
            `Name` varchar(45) NOT NULL,
            `Price` double NOT NULL,
            PRIMARY KEY (`idProduct`)
         );

         */
        public string Name { get; set; }
        public double Price { get; set; }
    }
}
