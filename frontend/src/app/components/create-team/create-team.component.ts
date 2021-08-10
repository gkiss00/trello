import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { User } from 'src/app/models/user';

import { TeamService } from 'src/app/services/team.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Team } from 'src/app/models/team';

@Component({
    templateUrl: 'create-team.component.html',
    styleUrls: ['create-team.component.css'],
})

export class CreateTeamComponent{
    teamId: number;
    allTeams: Team[] = [];
    @ViewChild("error") error: ElementRef;

    constructor(
        private teamService: TeamService,
        private router: Router,
        private authenticationService: AuthenticationService,
    ) {
        this.getAllTeams();
    }

    //GET THE NEXT ID
    getAllTeams(){
        this.teamService.getAllTeams().subscribe(teams => {
            this.allTeams = teams;
            if (this.allTeams.length == 0)
                this.teamId = 1;
            else
                this.teamId = this.allTeams[this.allTeams.length - 1].id + 1;
        });
    }

    //GET CURRENT USER
    get currentUser() {
        return this.authenticationService.currentUser;
    }

    setError(msg: String){
        this.error.nativeElement.innerHTML = msg;
    }

    //CREATE THE NEW TEAM
    createNewTeam(name: String){
        if (this.isNullOrEmpty(name) == true)
            return this.setError("Your team name can't be empty");
        if (this.validateName(name) == false)
            return this.setError("Your team name is already used");
        this.teamService.addTeam(this.teamId, name, this.currentUser.pseudo).subscribe(data => {
            if(data)
                this.router.navigate(["/team/" + this.teamId + "/members"]);
        })
    }

    //CHECK IF NAME IS EMPTY
    isNullOrEmpty(name: String) : boolean{
        var i = 0;
        if (name.length == 0)
            return (true);
        for (i = 0; i < name.length; ++i){
            if (name[i] != " ")
                return (false);
        }
        return (true);
    }

    //CHECK IF THE NAME IS ALREADY USED
    validateName(name: String){
        var i = 0;
        for (i = 0; i < this.allTeams.length; ++i){
            if (this.allTeams[i].name == name){
                return (false);
            }
        }
        return (true);
    }
}