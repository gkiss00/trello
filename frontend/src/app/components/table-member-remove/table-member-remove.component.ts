import { ElementRef, EventEmitter, HostListener, Input, OnInit, Output, ViewChild } from '@angular/core';
import { Component } from '@angular/core';
import { User } from 'src/app/models/user';

@Component({
    selector: 'table-member-remove',
    templateUrl: 'table-member-remove.component.html',
    styleUrls: ['table-member-remove.component.css']
})
export class TableMemberRemoveComponent implements OnInit{

    @Input() tableUsers: User[];
    @Output() closeInput = new EventEmitter<boolean>();
    @Output() userRemove = new EventEmitter<User>();
    @ViewChild("window") window: ElementRef;
    membersInput: boolean = false;

    constructor(
    ) {
    }

    ngOnInit(): void {
    }

    private displayUsername(user: User): string {
        return user.pseudo;
    }

    private deleteMember(user: User){
        this.userRemove.emit(user);
    }

    private stopPropagation(event){
        event.stopPropagation();
    }

    @HostListener('document:click', ['$event.target'])
    public onClick(targetElement) {
        if(!this.window.nativeElement.contains(targetElement)) {
            this.membersInput = false;
        }   
    }
   
}