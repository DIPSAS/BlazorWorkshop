namespace Workshop.Server
{
    public static class SeedData
    {
        public static void Initialize(DrugStoreContext db)
        {

            var deals = new DrugDeal[]
            {
                new DrugDeal()
                {
                    Name = "The pain reliever",
                    Description = "It makes the booboo go away!",
                    BasePrice = 20.00m,
                    ImageUrl = "img/drugs/vicodin.jpg",
                },
                new DrugDeal()
                {
                    Name = "The Party Pack",
                    Description = "It has all the drugs!",
                    BasePrice = 99.99m,
                    ImageUrl = "img/drugs/pill-sampler.jpg",
                },
                new DrugDeal()
                {
                    Name = "Sweet Tooth™ Insulin",
                    Description = "Control your diabetes in 1-2-3",
                    BasePrice = 25.00m,
                    ImageUrl = "img/drugs/insulin.jpg",
                },
                new DrugDeal()
                {
                    Name = "Purple Drank",
                    Description = "Soothing action for your head cold",
                    BasePrice = 49.00m,
                    ImageUrl = "img/drugs/purple-drank.jpg",
                },
                new DrugDeal()
                {
                    Name = "Off-brand Opioids",
                    Description = "The scourge of the midwest",
                    BasePrice = 50.00m,
                    ImageUrl = "img/drugs/pain-killers.jpg",
                },
                new DrugDeal()
                {
                    Name = "Holistic Placebos",
                    Description = "Beloved by Facebook aunts",
                    BasePrice = 75.00m,
                    ImageUrl = "img/drugs/natural.jpg",
                }
            };

            db.Deals.AddRange(deals);
            db.SaveChanges();
        }
    }
}
