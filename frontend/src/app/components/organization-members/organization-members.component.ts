import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/models/user';

import { OrganizationService } from 'src/app/services/organization.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Organization } from 'src/app/models/organization';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { UserService } from 'src/app/services/user.service';

@Component({
    templateUrl: 'organization-members.component.html',
    styleUrls: ['organization-members.component.css'],
})

export class OrganizationMembersComponent{
    users: User[] = [];
    orgId : number;
    organization : Organization;
    currentUser : User;
    add_user : FormGroup;
    ctlUsers : FormControl;
    allUsers: User[] = [];

    public userAlreadyIn : boolean = false;
    public userNotExist : boolean = false;

    constructor(
        private fb : FormBuilder,
        private orgService: OrganizationService,
        private userService: UserService,
        private authenticationService: AuthenticationService,
        private activatedRoute : ActivatedRoute,
        private router: Router,
    ) {
        this.currentUser = this.getCurrentUser;

        this.orgId = parseInt(this.activatedRoute.snapshot.paramMap.get('id'));

        orgService.getOrganization(this.orgId).subscribe(organization => {
            this.organization = organization;
            orgService.getCurrentMembers(this.orgId).subscribe(users => {
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

        this.userService.getAllUsers().subscribe(
            users => {
                this.allUsers = users;
            },
            error => {
                this.router.navigate(['/unknown']);
        });
        this.ctlUsers = this.fb.control('', []);
        this.add_user = this.fb.group({
            users : this.ctlUsers
        });
        
    }
    
    get getCurrentUser() {
        return this.authenticationService.currentUser;
    }


    titleUrlConverter(title: string): string{
        return title.replace(/\s/g, "-");
    }

    deleteUser(user)
    {
        if (this.users.length > 1)
            this.router.navigate(["/organization/confirm_delete/" + this.orgId + "/" + this.titleUrlConverter(user.pseudo)]);
        else 
            this.deleteOrga();
    }

    leaveOrga()
    {
        if (this.users.length > 1)
            this.router.navigate(["/organization/confirm_delete/" + this.orgId + "/" + this.titleUrlConverter(this.currentUser.pseudo)]);
        else
            this.deleteOrga();
    }

    deleteOrga()
    {
        this.router.navigate(["/organization/confirm_delete/" + this.orgId]);
    }

    back() 
    {
        this.router.navigate(["/table"]);
    }

    addUser(){
        this.userAlreadyIn = false;
        this.userNotExist = false;
        var input = this.ctlUsers.value;
        this.ctlUsers.setValue('');
        
        var user = this.allUsers.find(u => u.pseudo===input)
        if (user)
        {
            var uIn = this.users.find(u => u.pseudo===input)
            if (uIn)
                this.userAlreadyIn = true;
            else {
                this.users.push(user);
                this.add(user.id, input);
            }
        }
        else
            this.userNotExist = true;
    }

    add(id, input) {
        this.orgService.addUserToOrga(id, this.orgId).subscribe(
            u => {},
            error => {
                this.router.navigate(["/unknown"]);
            });
    }
}