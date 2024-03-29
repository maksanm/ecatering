﻿namespace ECaterer.Web.DTO.MealsDTO
{
    public class MealDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string[] AllergentList { get; set; }
        public string[] IngredientList { get; set; }
        public int Calories { get; set; }
        public bool Vegan { get; set; }
    } 
}
