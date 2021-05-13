using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using CarDealer.XMLHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            CarDealerContext context = new CarDealerContext();

            //context.Database.EnsureDeleted();
            //Console.WriteLine("Successfully deleted the database!");

            //context.Database.EnsureCreated();
            //Console.WriteLine("Successfully created the database!");

            //string xml1 = File.ReadAllText(@"../../../Datasets/suppliers.xml");
            //string xml2 = File.ReadAllText(@"../../../Datasets/parts.xml");
            //string xml3 = File.ReadAllText(@"../../../Datasets/cars.xml");
            //string xml4 = File.ReadAllText(@"../../../Datasets/customers.xml");
            //string xml5 = File.ReadAllText(@"../../../Datasets/sales.xml");

            //string result1 = ImportSuppliers(context, xml1);
            //Console.WriteLine(result1);

            //string result2 = ImportParts(context, xml2);
            //Console.WriteLine(result2);

            //string result3 = ImportCars(context, xml3);
            //Console.WriteLine(result3);

            //string result4 = ImportCustomers(context, xml4);
            //Console.WriteLine(result4);

            //string result5 = ImportSales(context, xml5);
            //Console.WriteLine(result5);


            var result = GetTotalSalesByCustomer(context);

            File.WriteAllText("../../../result.xml", result);
        }

        //Problem 09
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var suppliersDtos = XmlConverter.Deserializer<ImportSupplierDTO>(inputXml, "Suppliers");

            var result = suppliersDtos.Select(s => new Supplier
            {
                Name = s.Name,
                IsImporter = s.IsImporter
            })
            .ToArray();

            context.Suppliers.AddRange(result);
            context.SaveChanges();

            return $"Successfully imported {result.Length}";
        }

        //Problem 10
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var partsDtos = XmlConverter.Deserializer<ImportPartDTO>(inputXml, "Parts");

            var result = partsDtos
                .Where(i=>context.Suppliers.Any(s=>s.Id == i.SupplierId))
                .Select(p => new Part
                {
                Name = p.Name,
                Price = p.Price,
                Quantity = p.Quantity,
                SupplierId = p.SupplierId
                })
                .ToArray();

            context.Parts.AddRange(result);
            context.SaveChanges();

            return $"Successfully imported {result.Length}";
        }

        //Problem 11
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var carsDtos = XmlConverter.Deserializer<ImportCarDTO>(inputXml, "Cars");

            List<Car> cars = new List<Car>();

            foreach (var carDto in carsDtos)
            {
                var uniqueParts = carDto.Parts.Select(p => p.Id).Distinct().ToArray();
                var realParts = uniqueParts.Where(id => context.Parts.Any(i => i.Id == id));

                var car = new Car
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TravelledDistance = carDto.TravelledDistance,
                    PartCars = realParts.Select(id => new PartCar
                    {
                        PartId = id
                    }) 
                    .ToArray()
                };

                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        //Problem 12
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var customerDtos = XmlConverter.Deserializer<ImportCustomerDTO>(inputXml, "Customers");

            var customers = customerDtos.Select(x => new Customer
            {
                Name = x.Name,
                IsYoungDriver = x.IsYoungDriver,
                BirthDate = DateTime.Parse(x.BirthDate)
            })
            .ToArray();

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Length}";
        }

        //Problem 13
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            var salesDtos = XmlConverter.Deserializer<ImportSaleDTO>(inputXml, "Sales");

            var sales = salesDtos
                .Where(i => context.Cars.Any(x => x.Id == i.CarId))
                .Select(x => new Sale
                {
                    CarId = x.CarId,
                    CustomerId = x.CustomerId,
                    Discount = x.Discount
                })
                .ToArray();

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Length}";
        }

        //Problem 14
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var result = context.Cars
                .Where(c => c.TravelledDistance > 2000000)
                .Select(c => new ExportCarsWithDistanceDTO
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ToArray();

            var xmlResult = XmlConverter.Serialize(result, "cars");

            return xmlResult;
        }

        //Problem 15
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var result = context.Cars
                .Where(c => c.Make.Equals("BMW"))
                .Select(c => new ExportCarDTO
                {
                    Id = c.Id,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                })
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .ToArray();

            var xmlResult = XmlConverter.Serialize(result, "cars");

            return xmlResult;
        }

        //Problem 16
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var result = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new ExportSupplierDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToArray();

            var xmlResult = XmlConverter.Serialize(result, "suppliers");

            return xmlResult;
        }

        //Problem 17
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var result = context.Cars
                .Select(c => new ExportCarDTO
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                    Parts = context.Parts
                    .Where(p => c.PartCars.Any(cp => cp.PartId == p.Id))
                    .Select(p => new ExportPartDTO
                    {
                        Name = p.Name,
                        Price = p.Price
                    })
                    .OrderByDescending(p => p.Price)
                    .ToArray()
                })
                .OrderByDescending(c => c.TravelledDistance)
                .ThenBy(c => c.Model)
                .ToArray();

            var xmlResult = XmlConverter.Serialize(result, "cars");

            return xmlResult;
        }

        //Problem 18
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            //Get all customers that have bought at least 1 car and get their names, bought cars count and total spent money on cars. 
            //Order the result list by total spent money descending.

            var result = context.Sales
                .Select(s => new ExportCustomerDTO
                {
                    Name = s.Customer.Name,
                    BoughtCars = s.Customer.Sales.Count,
                    SpentMoney = context.PartCars
                    .Where(pc => pc.CarId == s.CarId)
                    .Sum(pc => pc.Part.Price)
                })
                .OrderByDescending(s => s.SpentMoney)
                .ToArray();

            var xmlResult = XmlConverter.Serialize(result, "customers");

            return xmlResult;
        }

        //Problem 19
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var result = context.Sales
                .Select(s => new ExportSaleDTO
                {
                    Car = new ExportCarDTO
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TravelledDistance = s.Car.TravelledDistance
                    },
                    Discount = s.Discount,
                    CustomerName = s.Customer.Name,
                    Price = s.Car.PartCars.Sum(p => p.Part.Price),
                    PriceWithDiscount = s.Car.PartCars.Sum(p => p.Part.Price) - 
                                        s.Car.PartCars.Sum(p => p.Part.Price) * s.Discount / 100,
                })
                .ToArray();

            var xmlResult = XmlConverter.Serialize(result, "sales");

            return xmlResult;
        }
    }
}