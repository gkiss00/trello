import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router'; 
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';

import { AuthenticationService } from '../../services/authentication.service';

@Component({
    templateUrl: 'signup.component.html',
    styleUrls: ['signup.component.css']
})
export class SignupComponent implements OnInit {
    signupForm: FormGroup;
    loading = false;    // utilisé en HTML pour désactiver le bouton pendant la requête de signup
    submitted = false;  // retient si le formulaire a été soumis ; utilisé pour n'afficher les 
    // erreurs que dans ce cas-là (voir template)
    returnUrl: string;
    ctlPseudo: FormControl;
    ctlPassword: FormControl;
    ctlPasswordConfirm: FormControl;
    ctlEmail: FormControl;
    ctlFirstName: FormControl;
    ctlLastName: FormControl;
    //ctlAge: FormControl;

    constructor(
        private formBuilder: FormBuilder,
        private route: ActivatedRoute,
        private router: Router,
        private authenticationService: AuthenticationService
    ) {
        // redirect to home if already logged in
        if (this.authenticationService.currentUser) {
            this.router.navigate(['/']);
        }
    }

    ngOnInit() {
        /**
         * Ici on construit le formulaire réactif. On crée un 'group' dans lequel on place sept
         * 'controls'. Remarquez que la méthode qui crée les controls prend comme paramêtre une
         * valeur initiale et un tableau de validateurs. Les validateurs vont automatiquement
         * vérifier les valeurs encodées par l'utilisateur et reçues dans les FormControls grâce
         * au binding, en leur appliquant tous les validateurs enregistrés. Chaque validateur
         * qui identifie une valeur non valide va enregistrer une erreur dans la propriété
         * 'errors' du FormControl. Ces erreurs sont accessibles par le template  grâce au binding.
         */
        this.ctlPseudo = this.formBuilder.control('', Validators.required);
        this.ctlPassword = this.formBuilder.control('', Validators.required);
        this.ctlPasswordConfirm = this.formBuilder.control('', Validators.required);
        this.ctlEmail = this.formBuilder.control('', Validators.required);
        this.ctlFirstName = this.formBuilder.control('');
        this.ctlLastName = this.formBuilder.control('');
        this.signupForm = this.formBuilder.group({
            pseudo: this.ctlPseudo,
            password: this.ctlPassword,
            passwordConfirm: this.ctlPasswordConfirm,
            email: this.ctlEmail,
            firstname: this.ctlFirstName,
            lastname: this.ctlLastName,
        });

        // get return url from route parameters or default to '/'
        this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/table';
    }

    // On définit ici un getter qui permet de simplifier les accès aux champs du formulaire dans le HTML
    get f() { return this.signupForm.controls; }

    clearPseudo(){
        this.signupForm.controls.pseudo.setValue("");
    }

    clearPassword(){
        this.signupForm.controls.password.setValue("");
    }

    clearPasswordConfirm(){
        this.signupForm.controls.passwordConfirm.setValue("");
    }

    clearEmail(){
        this.signupForm.controls.email.setValue("");
    }

    clearFirstName(){
        this.signupForm.controls.firstname.setValue("");
    }

    clearLastName(){
        this.signupForm.controls.lastname.setValue("");
    }

    /**
     * Cette méthode est bindée sur l'événement onsubmit du formulaire. On va y faire le
     * signup en faisant appel à AuthenticationService.
     */
    onSubmit() {
        this.submitted = true;

        // on s'arrête si le formulaire n'est pas valide
        if (this.signupForm.invalid) return;

        this.loading = true;
        this.authenticationService.signup(this.f.pseudo.value, this.f.password.value, this.f.passwordConfirm.value, this.f.email.value, this.f.firstname.value, this.f.lastname.value)
            .subscribe(
                // si login est ok, on navigue vers la page demandée
                data => {
                    this.router.navigate([this.returnUrl]);
                },
                // en cas d'erreurs, on reste sur la page et on les affiche
                error => {
                    const errors = error.error.errors;
                    for (let field in errors) {
                        this.signupForm.get(field.toLowerCase()).setErrors({ custom: errors[field] })
                    }
                    this.loading = false;
                });
    }
}
