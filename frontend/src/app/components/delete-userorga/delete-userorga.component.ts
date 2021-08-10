import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/models/user';

import { OrganizationService } from 'src/app/services/organization.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { Organization } from 'src/app/models/organization';

@Component({
    templateUrl: 'delete-userorga.component.html',
    styleUrls: ['delete-userorga.component.css'],
})
export class DeleteUserOrgaComponent{
    
    public pseudo : string;
    public orga : Organization;
    public orgId : number;
    public currentUser : User;

    constructor(
        private orgService: OrganizationService,
        private authenticationService: AuthenticationService,
        private activatedRoute : ActivatedRoute,
        private router: Router,
    ) {
        this.currentUser = this.getCurrentUser;
        this.pseudo = this.activatedRoute.snapshot.paramMap.get('pseudo');
        this.orgId = parseInt(this.activatedRoute.snapshot.paramMap.get('id'));
        this.orgService.getOrganization(this.orgId).subscribe(
            o => {
                this.orga = o
                this.orgService.getCurrentMembers(this.orga.id).subscribe(
                    m => {
                        var found = false;
                        m.forEach(u => {
                            if (u.pseudo === this.pseudo)
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
    }    
    
    get getCurrentUser() {
        return this.authenticationService.currentUser;
    }


    titleUrlConverter(title: string): string{
        return title.replace(/\s/g, "-");
    }

    back(){
        this.router.navigate(["/organization/" + this.orgId + "/members"]);
    }

    delete() {
        this.orgService.removeUser(this.pseudo, this.orgId)
        .subscribe(
            o => {
                this.router.navigate(["/organization/" + this.orgId + "/members"]);
            },
            error => {
                this.router.navigate(['/unknown']);
            });
    }
}