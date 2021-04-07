export interface UserGetDTO {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
}

export interface UserPostDTO {
  firstName: string;
  lastName: string;
  email: string;
  role: string | null;
}

export interface UserPatchDTO {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
}

export interface UserPasswordPatchDTO {
  currentPassword: string;
  newPassword: string;
}

export interface UserRolePatchDTO {
  role: string;
}


