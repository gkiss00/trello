import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';

@Component({
    selector: 'app-unknown',
    template: `
    <div class="main">
        <h1>This page doesn't exist...</h1>
        <p>Don't know what you're trying to access.. but it doesn't exist.</p>
        <iframe src="https://thumbs.gfycat.com/ShamefulSeparateBushsqueaker-small.gif" width="480" height="270" frameBorder="0" allowFullScreen></iframe>
    </div>
    `,
    styleUrls: ['unknown.component.css']
})

export class UnknownComponent implements OnInit {
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