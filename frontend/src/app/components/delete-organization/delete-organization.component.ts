import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/models/user';

import { OrganizationService } from 'src/app/services/organization.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Organization } from 'src/app/models/organization';

@Component({
    templateUrl: 'delete-organization.component.html',
    styleUrls: ['delete-organization.component.css'],
})

export class DeleteOrganizationComponent {
    currentUser : User;
    public orgId : number;
    public orga : Organization;

    constructor(
        private orgService: OrganizationService,
        private authenticationService: AuthenticationService,
        private activatedRoute : ActivatedRoute,
        private router: Router,
    ) {
        this.currentUser = this.getCurrentUser;
        this.orgId = parseInt(this.activatedRoute.snapshot.paramMap.get('id'));
        this.orgService.getOrganization(this.orgId)
        .subscribe(o => {
            this.orga = o;
            this.orgService.getCurrentMembers(this.orga.id).subscribe(
                m => {
                    var found = false;
                    m.forEach(u => {
                        if (u.pseudo === this.currentUser.pseudo)
                            found = true;
                    });
                    if (!found)
                        this.back();
                },
                error => {
                    this.router.navigate(['/unknown']);
            });
        });
    }
    
    get getCurrentUser() {
        return this.authenticationService.currentUser;
    }

    back(){
        this.router.navigate(["/table/"]);
    }

    delete() {
        this.orgService.delete(this.orgId)
        .subscribe(
            o => {
                this.router.navigate(["/table"]);
            },
            error => {
                this.router.navigate(['/unknown']);
            });
    }

    titleUrlConverter(title: string): string{
        return title.replace(/\s/g, "-");
    }

}