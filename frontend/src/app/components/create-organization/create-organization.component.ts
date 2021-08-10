import {Observable} from 'rxjs';
import { Router, ActivatedRoute } from '@angular/router';
import {map, startWith} from 'rxjs/operators';
import { Component, Input, OnInit} from '@angular/core';
import { FormControlName, FormGroup } from '@angular/forms';
import { FormControl } from '@angular/forms';
import { FormBuilder } from '@angular/forms';
import { Validators } from '@angular/forms';
import { forEach } from 'lodash-es';
import { Organization } from 'src/app/models/organization';
import { Table } from 'src/app/models/table';
import { Team } from 'src/app/models/team';
import { User } from 'src/app/models/user';
import { UserService } from 'src/app/services/user.service';
import { TeamService } from 'src/app/services/team.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { OrganizationService } from 'src/app/services/organization.service';


@Component({
    selector: 'create-organization',
    templateUrl: 'create-organization.component.html',
    styleUrls: ['create-organization.component.css']
})

export class CreateOrganizationComponent implements OnInit {

    public create_org: FormGroup;
    public ctlTitle : FormControl;
    public ctlTeams : FormControl;
    public ctlUsers : FormControl;
    public users : User[] = [];
    public teams : Team[] = [];
    public usersToAdd : User[] = [];
    public teamsToAdd : Team[] = [];
    public teamsToDisplay : string[] = [];
    public usersToDisplay : string[] = [];
    public currentUser : User;
    private usersDeleted : User[] = [];

    public userAlreadyIn : boolean = false;
    public userNotExist : boolean = false;
    public selfAdded : boolean = false;
    public userAlreadyInATeam : boolean = false;

    ngOnInit() {
    }
    
    constructor(private fb: FormBuilder,
        private route: ActivatedRoute,
        private router: Router,
        private userService: UserService,
        private teamService: TeamService,
        private orgnizationService: OrganizationService,
        private authenticationService: AuthenticationService
    ) {
        this.currentUser = this.getCurrentUser;
        userService.getCurrentUserTeamsWithOrga().subscribe(
            teams => {
                var res : Team[] = [];
                teams.forEach(t => {
                    if (t.organization == null || typeof(t.organization) == undefined) {
                        res.push(t);
                    }
                },
            error => {
                    this.router.navigate(['/unknown']);
                });
                this.teams = res;
        });
        this.userService.getAllUsers().subscribe(
            users => {
                this.users = users;
                var user = users.find(u => u.id == this.currentUser.id);
                const index = this.users.indexOf(user, 0);
                this.users.splice(index, 1);

                this.usersToDisplay.push(this.currentUser.pseudo);
            },
            error => {
                this.router.navigate(['/unknown']);
        });

        this.ctlTitle = this.fb.control('', [
            Validators.required,
            Validators.minLength(3),
            Validators.maxLength(20)
        ]);
        this.ctlTeams = this.fb.control('', []);
        this.ctlUsers = this.fb.control('', []);
        this.create_org = this.fb.group({
            title : this.ctlTitle,
            teams : this.ctlTeams,
            users : this.ctlUsers
        });
    }    
    
    get getCurrentUser() {
        return this.authenticationService.currentUser;
    }

    get f() {return this.create_org.controls;}
    
    addUser() {
        this.userAlreadyIn = false;
        this.userNotExist = false;
        this.userAlreadyInATeam = false;
        this.selfAdded = false;
        var input = this.ctlUsers.value;
        this.ctlUsers.setValue('');
        var user = this.users.find(u => u.pseudo===input);
        if (input === this.currentUser.pseudo)
        {
            this.selfAdded = true;
        }
        else if (user) {
            var found = false;
            if (this.teamsToAdd.length > 0) {
                this.teamsToAdd.forEach(t => {
                    t.usersIn.forEach(u => {
                        if (u.pseudo === input) {
                            this.userAlreadyInATeam = true;
                            found = true;
                        }
                    });
                });
            }
            if (!found) {
                this.usersToAdd.push(user);
                this.usersToDisplay.push(user.pseudo);
                const index = this.users.indexOf(user, 0);
                this.users.splice(index, 1);
            }
        }
        else {
            user = this.usersToAdd.find(u => u.pseudo===input);
            if (user)
                this.userAlreadyIn = true;
            else {
                this.userNotExist = true;
            }
        }
    }    
    
    addTeam() {
        var input = this.ctlTeams.value;
        this.ctlTeams.setValue('');
        var team = this.teams.find(t => t.id===input);

        this.teamService.getCurrentMembers(team.id).subscribe(
            usersIn => {
                team.usersIn = usersIn;
                this.teamsToDisplay.push(team.name);
                this.teamsToAdd.push(team);
                const index = this.teams.indexOf(team, 0);
                this.teams.splice(index, 1);
                // check si un user faisant partie de la team est a enlever
                if (this.usersToAdd.length > 0 && usersIn.length > 0){
                    usersIn.forEach(u => {
                        var user = this.usersToAdd.find(t_u => t_u.pseudo === u.pseudo);
                        if (user)
                        {
                            this.deleteUser(user.pseudo);
                            this.usersDeleted.push(user);
                        }
                    });
                }
            },
            error => {
                this.router.navigate(['/unknown']);
        });
    }

    deleteUser(pseudo) {
        var user = this.usersToAdd.find(u => u.pseudo===pseudo);
        var index = this.usersToAdd.indexOf(user, 0);
        this.usersToAdd.splice(index, 1);
        index = this.usersToDisplay.indexOf(pseudo, 0);
        this.usersToDisplay.splice(index, 1);
        this.users.push(user);
    }


    deleteTeam(name) {
        var team = this.teamsToAdd.find(t => t.name===name)
        var index = this.teamsToAdd.indexOf(team, 0);
        this.teamsToAdd.splice(index, 1);
        index = this.teamsToDisplay.indexOf(name, 0);
        this.teamsToDisplay.splice(index, 1);
        this.teams.push(team);

        // check dans usersDeleted si faisait partie de la team delete ou pas
        this.teamService.getCurrentMembers(team.id).subscribe(
            usersIn => {
                if (this.usersDeleted.length > 0 && usersIn.length > 0){
                    usersIn.forEach(cur_u => {
                        var cpy = this.usersDeleted;
                        cpy.forEach(u => {
                            if (u.pseudo == cur_u.pseudo)
                            {
                                this.usersToAdd.push(u);
                                this.usersToDisplay.push(u.pseudo);
            
                                const index = this.usersDeleted.indexOf(u, 0);
                                this.usersDeleted.splice(index, 1);
                            }
                        });
                    });
                }
            },
            error => {
                this.router.navigate(['/unknown']);
        });
    }

    create() {
        if (this.create_org.invalid) return;

        this.orgnizationService.create(this.f.title.value, this.usersToDisplay, this.teamsToAdd)
            .subscribe(
                o => {
                    this.router.navigate(["/organization/" + o.id + "/members"]);
                
                },
                error => {
                    this.router.navigate(['/unknown']);
            });
    }
}