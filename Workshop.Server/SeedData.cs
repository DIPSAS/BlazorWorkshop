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
                    BasePrice = 9.99m,
                    ImageUrl = "img/drugs/vicodin.jpg",
                },
                new DrugSpecial()
                {
                    Id = 2,
                    Name = "The collection",
                    Description = "It has all the drugs!",
                    BasePrice = 11.99m,
                    ImageUrl = "img/drugs/pill-sampler.jpg",
                },
                new DrugSpecial()
                {
                    Id = 3,
                    Name = "The big one",
                    Description = "Control your diabetes in 1-2-3",
                    BasePrice = 10.50m,
                    ImageUrl = "img/drugs/insulin.jpg",
                }
            };

            db.Specials.AddRange(specials);
            db.SaveChanges();
        }
    }
}
