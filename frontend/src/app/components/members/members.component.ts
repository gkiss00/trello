import { Component, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { User } from 'src/app/models/user';

import { TeamService } from 'src/app/services/team.service';
import { UserService } from 'src/app/services/user.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Team } from 'src/app/models/team';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';

@Component({
    templateUrl: 'members.component.html',
    styleUrls: ['members.component.css'],
})

export class MembersComponent{
    users: User[] = [];
    teamId : number;
    team : Team;
    allUsers: User[] = [];
    @ViewChild("error") error: ElementRef;

    constructor(
        @Inject('BASE_URL') private baseUrl: string,
        private formBuilder: FormBuilder,
        private teamService: TeamService,
        private userService: UserService,
        private router: Router,
        private authenticationService: AuthenticationService,
        private activatedRoute : ActivatedRoute
    ) {
        this.teamId = parseInt(this.activatedRoute.snapshot.paramMap.get('id'));

        teamService.getCurrentMembers(this.teamId).subscribe(
            users => {
                this.users = users;
            },
            error => {
                this.router.navigate(['/restricted']);
        });

        teamService.getTeam(this.teamId).subscribe(
            team => {
                this.team = team;
            },
            error => {
                this.router.navigate(['/restricted']);
        });

        userService.getAllUsers().subscribe(
            us =>{
                this.allUsers = us;
            },
            error => {
                this.router.navigate(['/restricted']);
        })
        
    }

    get currentUser() {
        return this.authenticationService.currentUser;
    }

    
    //ADD MEMBER TO THE TEAM
    addMember(pseudo: String){
        if (this.isNullOrEmpty(pseudo) == true)
            return this.setError("The pseudo can't be empty");
        else if (this.isAlreadyIn(pseudo) == true)
            return this.setError("This member is already part of the team");
        else if (this.doNotExist(pseudo) == true)
            return (this.setError("This user doesn't exist"));
        else
            this.error.nativeElement.innerHTML = "";
        this.teamService.addMember(this.teamId, "add", pseudo).subscribe(data => {
            this.users.push(data);
        });
    }

    setError(msg: String){
        this.error.nativeElement.innerHTML = msg;
    }

    //CHECK IF NME IS EMPTY
    isNullOrEmpty(pseudo: String) : boolean{
        var i = 0;
        if (pseudo.length == 0)
            return (true);
        for (i = 0; i < pseudo.length; ++i){
            if (pseudo[i] != " ")
                return (false);
        }
        return (true);
    }
    //CHECK IF THE USER IS ALREADY IN THE TEAM
    isAlreadyIn(pseudo: String){
        var i = 0;
        for (i = 0; i < this.users.length; ++i){
            if (pseudo == this.users[i].pseudo)
                return (true);
        }
        return (false);
    }

    doNotExist(pseudo: String){
        var i = 0;
        for (i = 0; i < this.allUsers.length; ++i){
            if (pseudo == this.allUsers[i].pseudo)
                return (false);
        }
        return (true);
    }

    leaveTeam(){
        this.teamService.removeMember(this.teamId, "remove", this.currentUser.pseudo).subscribe(data => {
            this.router.navigate(["/table"]);
        });
    }
}