import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/models/user';

import { TeamService } from 'src/app/services/team.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Team } from 'src/app/models/team';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';

@Component({
    templateUrl: 'delete-team.component.html',
    styleUrls: ['delete-team.component.css'],
})

export class DeleteTeamComponent {
    teamId: number;
    team : Team;

    constructor(
        private formBuilder: FormBuilder,
        private teamService: TeamService,
        private router: Router,
        private authenticationService: AuthenticationService,
        private activatedRoute : ActivatedRoute
    ) {
        this.teamId = parseInt(this.activatedRoute.snapshot.paramMap.get('id'));

        teamService.getTeam(this.teamId).subscribe(
            team => {
                this.team = team;
            },
            error => {
                this.router.navigate(['/restricted']);
        });
    }

    //GET CURRENT USER
    get currentUser() {
        return this.authenticationService.currentUser;
    }

    delete(){
        this.teamService.deleteTeam(this.teamId).subscribe(
            data => {
                this.cancel();
            },
            error => {
                this.router.navigate(['/unknown']);
        });
    }

    cancel(){
        this.router.navigate(["/table"]);
    }
}