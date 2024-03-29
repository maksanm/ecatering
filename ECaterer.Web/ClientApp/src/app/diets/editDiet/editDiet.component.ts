import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute, Params, Router } from "@angular/router";
import { DietsService } from '../api/diets.service';
import { RegistrationService } from '../../registration/api/registration.service';
import { ProducerDietDTO } from '../api/producerDietDTO';
import { mealDto } from '../../meals/api/mealsDtos';
import { MealsService } from '../../meals/api/meals.service';
import { EditDietDTO } from '../api/editDietDTOs';

@Component({
  selector: 'app-edit-diet',
  templateUrl: './editDiet.component.html',
  styleUrls: ['./editDiet.component.scss'],
  encapsulation: ViewEncapsulation.None
})

export class EditDietComponent implements OnInit {
  constructor(private TitleService: Title, private router: Router, private dietsService: DietsService,
    private mealsService: MealsService,
    private registrationService: RegistrationService,
    private inRoute: ActivatedRoute) {
    this.TitleService.setTitle("Edycja diety");
  }

  private dietId: string;

  public uniqueMealId: number = 0;

  public addMealDialog: boolean = false;
  
  public previewMealDialog: boolean = false;

  public mealsInDiet: mealDto[] = [];

  public editModel: EditDietDTO = {
    id: "",
    price: 0,
    name: "",
    calories: 0,
    description: "",
    vegan: true,
    veganString: ""
  };

  public newMealData: mealDto = {
    id: "",
    name: "",
    allergentList: [],
    allergentString: "",
    ingredientList: [],
    ingredientString: "",
    calories: 0,
    vegan: true
  };

  public selectedMeal: mealDto;

  ngOnInit(): void {
    this.inRoute.params.subscribe((params: Params) => {
      this.dietId = params['id'];
      this.resolveMeals();
    });
  }

  resolveMeals() {
    this.mealsService.getMeals(this.dietId)
      .then((data) => {
        this.mealsInDiet.push(...(data as mealDto[]));
        this.updateCaloriesAndVegan();
      });
    this.dietsService.getEditModelDiet(this.dietId)
      .then((data) => {
        this.editModel = data as EditDietDTO;
        this.updateVeganString();
      });
  }

  updateCaloriesAndVegan() {
    this.editModel.vegan = this.mealsInDiet.every(m => m.vegan);
    this.editModel.calories = this.mealsInDiet.map(m => m.calories).reduce(
      (prev, curr) => prev + curr, 0
    );
  }

  updateVeganString() {
    this.editModel.veganString = this.editModel.vegan ? 'Tak' : 'Nie';
  }

  public openMealDialog() {
    this.addMealDialog = true;
  }

  public close() {
    this.addMealDialog = false;
  }
  
  public saveNewMeal() {
    if (this.newMealData.name.length == 0) {
      alert("Nazwa posiłku nie może być pusta");
      return;
    }
    this.newMealData.allergentList = this.newMealData.allergentString.split(';');
    this.newMealData.ingredientList = this.newMealData.ingredientString.split(';');
    // this is temporary id for uniqueness while removing
    this.newMealData.id = this.uniqueMealId.toString();
    this.uniqueMealId++;
    this.mealsInDiet.push(Object.assign({}, this.newMealData));
    this.updateCaloriesAndVegan();
    this.updateVeganString();
    this.addMealDialog = false;
  }

  public removeFromDiet(mealId: string) {
    this.mealsInDiet = this.mealsInDiet.filter(m => m.id != mealId);
    this.updateCaloriesAndVegan();
    this.updateVeganString();
  }

  public previewMeal(mealId: string) {
    this.selectedMeal = this.mealsInDiet.find(m => m.id == mealId);
    this.previewMealDialog = true;
  }

  public closePreview() {
    this.previewMealDialog = false;
  }

  public saveDiet() {
    if (this.mealsInDiet.length == 0) {
      alert("Dieta nie może być pusta");
      return;
    }
    this.dietsService.editDiet(this.mealsInDiet, this.editModel)
      .then((data) => {
        this.router.navigate(["/producer/diets"]);
      });
  }
}
