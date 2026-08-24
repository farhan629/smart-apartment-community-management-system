import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { PermissionService } from '../../../../core/services/permission.service';
import { AmenityService, CreateAmenityRequestDto } from '../../../../core/services/aminety-service';
import { AddNewAmenity } from '../../components/add-new-amenity/add-new-amenity';
import { PERMISSIONS } from '../../../../core/constants/permission.constants';
import { ADD_NEW_AMENITY_STRINGS, AMENITY_ROUTES } from '../../../../core/constants/amenity.constants';

@Component({
  selector: 'app-add-new-amenity-page',
  standalone: true,
  imports: [CommonModule, AddNewAmenity, MatSnackBarModule],
  templateUrl: './add-new-amenity-page.html',
  styleUrl: './add-new-amenity-page.scss',
})
export class AddNewAmenityPage implements OnInit {
  isSubmitting = signal<boolean>(false);
  pageStrings = ADD_NEW_AMENITY_STRINGS;

  constructor(
    private router: Router,
    private permissionService: PermissionService,
    private amenityService: AmenityService,
    private snackBar: MatSnackBar,
  ) {}

  ngOnInit(): void {
    if (!this.permissionService.hasPermission(PERMISSIONS.AMENITY_MANAGE)) {
      this.goBack();
    }
  }

  onSubmitAmenity(requestBody: CreateAmenityRequestDto): void {
    this.isSubmitting.set(true);

    this.amenityService.postApiAmenity(requestBody).subscribe({
      next: () => {
        this.snackBar.open(this.pageStrings.TOAST_SUCCESS, 'Close', {
          duration: 3000,
          horizontalPosition: 'right',
          verticalPosition: 'bottom',
          panelClass: ['success-snackbar']
        });
        this.goBack();
      },
      error: (err) => {
        console.error('Error creating amenity', err);
        alert(this.pageStrings.ERROR_SUBMIT);
        this.isSubmitting.set(false);
      }
    });
  }

  goBack(): void {
    this.router.navigate([AMENITY_ROUTES.BASE]);
  }
}
