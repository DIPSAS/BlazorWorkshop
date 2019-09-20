namespace Workshop.Server
{
    public static class SeedData
    {
        public static void Initialize(DrugStoreContext db)
        {

            var specials = new DrugSpecial[]
            {
                new DrugSpecial()
                {
                    Name = "The pain reliever",
                    Description = "It makes the booboo go away!",
                    BasePrice = 20.00m,
                    ImageUrl = "img/drugs/vicodin.jpg",
                },
                new DrugSpecial()
                {
                    Name = "The collection",
                    Description = "It has all the drugs!",
                    BasePrice = 99.99m,
                    ImageUrl = "img/drugs/pill-sampler.jpg",
                },
                new DrugSpecial()
                {
                    Name = "The big one",
                    Description = "Control your diabetes in 1-2-3",
                    BasePrice = 25.00m,
                    ImageUrl = "img/drugs/insulin.jpg",
                },
                new DrugSpecial()
                {
                    Name = "Purple drank",
                    Description = "Soothing action for your head cold",
                    BasePrice = 49.00m,
                    ImageUrl = "img/drugs/purple-drank.jpg",
                },
                new DrugSpecial()
                {
                    Name = "Off-brand opiods",
                    Description = "The scourge of the midwest",
                    BasePrice = 50.00m,
                    ImageUrl = "img/drugs/pain-killers.jpg",
                },
                new DrugSpecial()
                {
                    Name = "Holistic Placebos",
                    Description = "Beloved by Facebook aunts",
                    BasePrice = 75.00m,
                    ImageUrl = "img/drugs/natural.jpg",
                }
            };

            db.Specials.AddRange(specials);
            db.SaveChanges();
        }
    }
}
