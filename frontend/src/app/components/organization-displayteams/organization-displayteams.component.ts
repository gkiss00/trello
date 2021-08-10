import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/models/user';

import { OrganizationService } from 'src/app/services/organization.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Organization } from 'src/app/models/organization';
import { Team } from 'src/app/models/team';
import { TeamService } from 'src/app/services/team.service';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';

@Component({
    templateUrl: 'organization-displayteams.component.html',
    styleUrls: ['organization-displayteams.component.css'],
})

export class OrganizationDisplayTeamsComponent{
    public users: User[] = [];
    public orgId : number;
    public organization : Organization;
    public currentUser : User;
    public teams : Team[] = [];
    public teams_addable : Team[] = [];
    public add_team : FormGroup;
    public ctlTeams : FormControl;

    constructor(
        private fb : FormBuilder,
        private orgService: OrganizationService,
        private teamService: TeamService,
        private authenticationService: AuthenticationService,
        private activatedRoute : ActivatedRoute,
        private router: Router,
    ) {
        this.currentUser = this.getCurrentUser;

        this.orgId = parseInt(this.activatedRoute.snapshot.paramMap.get('id'));

        this.orgService.getOrganization(this.orgId).subscribe(
            organization => {
                this.organization = organization;
                orgService.getCurrentMembers(this.orgId).subscribe(
                    users => {
                        this.users = users;
                        var found = false;
                        users.forEach(u => {
                            if (u.pseudo === this.currentUser.pseudo)
                                found = true;
                        });
                        if (!found)
                            this.back();
                        },
                    error => {
                        this.router.navigate(['/unknown']);
                });
            },
            error => {
                this.router.navigate(['/unknown']);
        });

        this.orgService.getCurrentTeams(this.orgId).subscribe(t => {
            t.forEach(team => {
                this.teamService.getCurrentMembers(team.id).subscribe(m => {
                    team.usersIn = m;
                },
                error => {
                    this.router.navigate(['/unknown']);
                });
            },
            error => {
                this.router.navigate(['/unknown']);
            });
            this.teams = t;
        });

        this.orgService.getTeamsAddable(this.orgId).subscribe(t => {
            t.forEach(team => {
                this.teamService.getCurrentMembers(team.id).subscribe(m => {
                    team.usersIn = m;
                },
                error => {
                    this.router.navigate(['/unknown']);
                });
            },
            error => {
                this.router.navigate(['/unknown']);
            });
            this.teams_addable = t;
        },
        error => {
            this.router.navigate(['/unknown']);
        });
        
        this.ctlTeams = this.fb.control('', []);
        this.add_team = this.fb.group({
            teams : this.ctlTeams
        });
    }
    
    get getCurrentUser() {
        return this.authenticationService.currentUser;
    }


    titleUrlConverter(title: string): string{
        return title.replace(/\s/g, "-");
    }

    back() 
    {
        this.router.navigate(["/table"]);
    }

    remove(id)
    {
        this.router.navigate(["/organization/confirm_delete/" + this.orgId + "/team/" + id]);
    }
    
    addTeam() {
        var input = this.ctlTeams.value;
        this.ctlTeams.setValue('');
        if (input > 0){  
            var t = this.teams_addable.find(t => t.id===input);
            this.teams.push(t);
            const index = this.teams_addable.indexOf(t, 0);
            this.teams_addable.splice(index, 1);
            this.orgService.addTeamToOrga(this.orgId, t.id).subscribe(
                o => {},
                error => {
                    this.router.navigate(['/unknown']);
                });
        }
    }

}