import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormGroup, FormControl, Validators } from "@angular/forms";
import { TextBoxComponent } from "@progress/kendo-angular-inputs";
import { Title } from '@angular/platform-browser';
import { RegistrationService } from '../api/registration.service';
import { Router } from "@angular/router";

@Component({
  selector: 'app-client-registration',
  templateUrl: './client-registration.component.html',
  styleUrls: ['./client-registration.component.scss'],
  encapsulation: ViewEncapsulation.None
})

export class ClientRegistration implements OnInit {


  public phoneNumberMask = "+48-000-000-000";
  public passwordReg = "^(?=(.*[A-Z]){1,})(?=(.*[!@#$%^&*()+.]){1,})(?=(.*[0-9]){1,})(?=(.*[a-z]){1,}).{8,25}$";

  public registrationData: IRegistrationData = {
    name: "",
    surname: "",
    email: "",
    password: "",
    phone: ""
  }

  public errorMessage = '';

  public form: FormGroup;

  constructor(private TitleService: Title, private router: Router, private registrationService: RegistrationService) {

    this.TitleService.setTitle("Rejestracja");

    this.form = new FormGroup({
      name: new FormControl(this.registrationData.name, [Validators.required, Validators.maxLength(50), Validators.minLength(2)]),
      surname: new FormControl(this.registrationData.surname, [Validators.required, Validators.maxLength(50), Validators.minLength(2)]),
      email: new FormControl(this.registrationData.email, [Validators.required, Validators.email, Validators.maxLength(250)]),
      password: new FormControl(this.registrationData.password, [
        Validators.required, Validators.maxLength(25), Validators.pattern(this.passwordReg)]),
      phone: new FormControl(this.registrationData.phone, [Validators.required, Validators.maxLength(20)])
    });
}

  ngOnInit(): void {
    $("#passwordTextBox input").attr("type", "password");
  }


  submitForm(): void {
    this.form.markAllAsTouched();
    this.clearError();
    if (this.form.valid) {
      this.registrationService.registerUser()
        .then(() => {
          console.log("Pomyślnie zarejestrowano.");
        })
        .catch((err) => {
          this.showError(err);
        });
    }
    else {
      this.showError("Niepoprawne dane.");
    }
  }


  redirectToLogin(): void {
    this.router.navigate(['/client/login']);
  }

  clearError() {
    this.errorMessage = "";
  }

  showError(message: string) {
    this.errorMessage = message;
  }
}

export interface IRegistrationData {
  name: string,
  surname: string,
  email: string,
  password: string,
  phone: string
}
