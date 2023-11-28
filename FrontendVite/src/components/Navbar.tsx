import "../App.css";

import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthProvider';
import logo from '/logo.png';


export default function Navbar() {

    const { isLoggedIn, signOut } = useAuth();

    const pageLinks = [
        { name: 'Find Navn', path: '/find-navn' },
        { name: 'Alle Navne', path: '/navne' },
        { name: 'Vores Matches', path: '/', imgUrl: '/heart.svg'},
      ]

  return (
    <nav className='absolute top-0 w-full flex items-center justify-between border-b py-6 md:px-8 lg:px-28'>
      <div className='uppercase text-sm flex items-center'>
      <img src={logo} alt="logo" className='pr-10 cursor-pointer' onClick={
        () => {
          window.location.href = '/'
        }
      }/>
        {pageLinks.map((link) => (
          <div key={link.name} className='flex gap-4 '>
            <Link key={link.name} to={link.path} className='flex items-center gap-2'>
            {link.imgUrl && <img src={link.imgUrl} alt="heart" className='w-4 h-4 ' />}
            {link.name}
          </Link>
              <div className='h-5 w-px bg-gray-300 mr-4' />            
          </div>
        ))}
        {isLoggedIn ? (
              <div className='space-x-4'>
                <Link to='/profile'>
                Min profil
              </Link>
                <button onClick={signOut} className='border-button'>
                Log ud
              </button>
              </div>
            ) : (
              <div className='flex '>
                <Link to='/sign-in' className='border-button'>
                Log ind
              </Link>
              </div>
            )}
      </div>
      <div className='space-x-2 items-center hidden lg:flex'>
        <input type="text" placeholder="Søg navn" className='border py-2 px-4'/>
        <button className='border-button'>Søg</button>
      </div>
    </nav>
  )
}
