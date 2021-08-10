import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';

@Component({
    selector: 'app-restricted',
    template: `
    <h1>Restricted Access</h1>
    <p>You will be redirected automatically to the home page...</p>
    `
})

export class RestrictedComponent implements OnInit {
    constructor(private router: Router, private authenticationService: AuthenticationService) { }

    ngOnInit() {
        if(this.authenticationService.currentUser){
            setTimeout(() => {
                this.router.navigate(['/table']);
            }, 5000);
        } else {
            setTimeout(() => {
                this.router.navigate(['/']);
            }, 5000);
        }
    }
}